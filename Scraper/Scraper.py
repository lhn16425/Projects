import os
import re
import csv
import json
import time
import argparse
import string
from urllib.request import urlopen
from bs4 import BeautifulSoup
from Property import Property, Project

class Scraper:

	domain = 'https://www.domain.com.au'

	def __init__(self, url, name = None):
		
		if name is None:
			self.name = ''
			self.logPrefix = ''
		else:
			self.name = name
			self.logPrefix = name + ": "
	
		self.URL = url		
		self.pageUrlPattern = ''
		self.currentPage = 1
		self.maxPage = 1

		self.propertyNum = 0
		self.runningTotal = 0
		self.addressLocality = ''
		self.addressRegion = ''
		self.postalCode = ''

		self.propertyList = []		

	def __textBetween(self, string, leader, trailer):
		try:
			endOfLeader = string.index(leader) + len(leader)
			startOfTrailer = string.index(trailer, endOfLeader)
			return string[endOfLeader:startOfTrailer]
		except:
			return None

	def __log(self, string):
		print('{0}Page {1:<2d}: {2}'.format(self.logPrefix, self.currentPage, string), end='\n', flush=True)
		
	def __logNP(self, string):
		print('{0}{1}'.format(self.logPrefix, string), end='\n', flush=True)

	def __getData(self, dom):	
		listings = dom.find_all('li', class_='search-results__listing')
		recordsFound = 0		
		for listing in listings:
			if listing.find('div', class_='listing-result__standard-project'):
				self.__log('Found a project')
				property = self.__getProjectListing(listing)
			elif listing.find('div', class_='listing-result-redesign__standard-project'):
				self.__log('Found a redesign project')
				property = self.__getProjectListing(listing, True)
			elif listing.find('div', class_='adspot__wrapper'):
				# skip if this is an ad
				continue
			else:
				property = self.__getPropertyListing(listing)
			if property:
				if isinstance(property, Project):
					recordsFound += property.getChildPropertyCount()
				else:
					recordsFound += 1

				#
				# You can save the data directly to DB here instead of saving it in
				# propertyList and sending them to the DB later in saveData()
				#
				self.propertyList.append(property)

		self.runningTotal += recordsFound
		
		self.__log('{0:2d} records(s) found - running total: {1:3d} / {2}'.format(recordsFound, self.runningTotal, self.propertyNum))

	def __getPrice(self, priceText):
		# if priceText starts with 'High ...' then return 0 and get price from
		# home price guide
		if re.match(r'^High', priceText, re.IGNORECASE):
			return 0
		match = re.search(r'\$?(\d{1,3}(,?\d{1,3})*)\s*(K|k|M|m)?', priceText)
		if match:
			price = match.group(1).replace(',', '')
			if match.group(3):
				if match.group(3).lower() == 'k':
					price += '000'
				elif match.group(3).lower() == 'm':
					price += '000000'
			return int(price)
		return 0
		
	def __getPriceFromHomePriceGuide(self, address, locality, region, postalCode):
		address += '-%s-%s-%s' % (locality, region, postalCode)
		address = re.sub(r' ', r'-', address)
		address = re.sub(r'-+', r'-', address)
		address = address.lower()
		homePriceGuideUrl = '%s/property-profile/%s' % (self.domain, address)
		#print(homePriceGuideUrl)
		page = urlopen(homePriceGuideUrl)
		html = str(page.read())
		match = re.search(r'"midPrice":\s*(\d+)', html)
		if match:
			return int(match.group(1))
		else:
			return 0
		
	def __getPriceFromAddress(self, addressLine1, locality, region, postalCode) :
		# get rid of the range in the address, for example: 15-20 -> 15, 281 & 283 -> 281
		address = re.sub(r'\s*\-\s*\d+|\s*&\s*\d+', r'', addressLine1)
		
		# get alternate address, for example: 20B/300 Turton St -> 300 Turton St
		altAddress = re.sub(r'^[^\/]*\/\s*', r'', address)
		if (altAddress == address):
			# get rid of the first part of the address, for example: Unit 41, 4 Glenefer Street -> 4 Glenefer Street
			altAddress = re.sub(r'^[^,]*,\s*', r'', address)
			if (altAddress == address):
				altAddress = None
		
		# change the / or comma in the address to -
		address = re.sub(r'\/|,', r'-', address)
		
		try:
			price = self.__getPriceFromHomePriceGuide(address, locality, region, postalCode)		
			if price == 0 and altAddress:		
				price = self.__getPriceFromHomePriceGuide(altAddress, locality, region, postalCode)
		except:
			print("Could not get price from home price guide for the following address: %s, %s, %s, %s" % (addressLine1, locality, region, postalCode))
			price = 0

		return price
	
	def __makePrintable(self, s):
		if not s.isprintable():
			s1 = ''
			for c in s:			
				if c in string.printable:
					s1 += c
				else:
					#print(c)
					if c == '–':
						s1 += '-'
					elif c == '’':
						s1 += '\''
					elif c == '•':
						s1 += '*'
					elif c == '“':
						s1 += '"'
					elif c == '”':
						s1 += '"'
					elif c == 'é':
						s1 += 'e'
					elif c == 'Ã':
						s1 += 'A'
					elif c == 'ç':
						s1 += 'c'
					elif c == 'â':
						s1 += 'a'
					elif c == '§':
						s1 += '*'
					elif c == '':
						s1 += '*'
					else:
						s1 += '?'
						#self.__log('UNPRINTABLE: %c' % c)
			return s1
		return s
	
	def __getPropertyDescription(self, dom):
		description = ''
		script = str(dom.body.div.script).strip()
		if script:
			match = re.search(r'window.renderizrData\["page"\]\s*=\s*(\{.+\});</script>$', script)
			if match:
				propertyData = json.loads(match.group(1))
				if propertyData:
					if 'description' in propertyData:
						description = '\u000A'.join(propertyData['description'])
					if 'headline' in propertyData:
						description = "%s\n\n%s" % (propertyData['headline'], description)
		return self.__makePrintable(description)
		
	def __getPropertyListing(self, listing):
		priceNode = listing.find('p', class_='listing-result__price')
		if priceNode is None:
			priceNode = listing.find('p', class_='listing-result-redesign__price')
			if priceNode is None:
				print(str(listing))
				raise ValueError('No price tag found.')
		priceText = priceNode.text.strip()
		
		addressNode = listing.find('a', class_='listing-result__address')
		if addressNode is None:
			addressNode = listing.find('a', class_='listing-result-redesign__address')
		if addressNode and addressNode.has_attr('href'):
			propertyUrl = addressNode['href']
		else:
			print(str(listing))
			raise ValueError('No property URL found.')
			
		addressLine1Node = addressNode.find('span', class_='address-line1')
		if addressLine1Node:
			addressLine1 = addressLine1Node.text.strip()
			if addressLine1.endswith(','):
				addressLine1 = addressLine1[:-1]
		else:
			addressLine1 = ''

		addressLine2Node = addressNode.find('span', class_='address-line2')
		if addressLine2Node:
			addressLine2Parts = addressLine2Node.select('span')
			addressLocality = addressLine2Parts[0].text.strip()
			addressRegion = addressLine2Parts[1].text.strip()
			postalCode = addressLine2Parts[2].text.strip()
		else:
			print(str(listing))
			raise ValueError('No address line 2 found.')
					
		price = self.__getPrice(priceText)
		if price == 0 and addressLine1:
			price = self.__getPriceFromAddress(addressLine1, addressLocality, addressRegion, postalCode)
			
		property = Property(price, addressLine1, addressLocality, addressRegion, postalCode)
		property.setUrl(propertyUrl)
		
		page = urlopen(propertyUrl)
		dom = BeautifulSoup(page, 'html.parser')
		keyFeatureNodes = dom.find_all('div', class_='listing-details__key-features--item')
		#print(propertyUrl)
		for keyFeatureNode in keyFeatureNodes:
			key = keyFeatureNode.find('div', class_='listing-details__key-features--key').text.strip()
			value = keyFeatureNode.find('div', class_='listing-details__key-features--value').text.strip()
			#print(key, value)
			if re.search(r'Property type', key, re.IGNORECASE):
				property.setType(value)
			elif re.search(r'Land area', key, re.IGNORECASE):
				match = re.search(r'(\d+)', value)
				if match:
					#print(match.group(1))
					property.setLandArea(int(match.group(1)))
					
		description = self.__getPropertyDescription(dom)
		if description:			
			#print(propertyUrl)
			#print(description)
			property.setDescription(description)
		else:
			print(propertyUrl)
			raise ValueError('No description found.')
					
		if property.getType() == '':
			print(propertyUrl)
			raise ValueError('No property type found.')
			
		featureNodes = listing.find_all('span', class_='property-feature__feature-text-container')
		if len(featureNodes) == 0:
			print(str(listing))
			raise ValueError('No feature found.')

		for featureNode in featureNodes:
			property.setFeature(featureNode.text.strip())
			
		return property
	
	def __getProjectListing(self, listing, isRedesign = False):
	
		#print(str(listing))

		if isRedesign:
			addressNodeClass = 'listing-result-redesign__project-address'
			titleNodeClass = 'listing-result-redesign__project-title'
			urlNodeClass = 'listing-result-redesign__project-title-wrapper'
			featureNodeClass = 'listing-result-redesign__project-features'
			childNodeClass = 'listing-result-redesign__listing'
			priceNodeClass = 'listing-result-redesign__price'
		else:
			addressNodeClass = 'listing-result__project-address'
			titleNodeClass = 'listing-result__project-title'
			urlNodeClass = 'listing-result__project-title-wrapper'
			featureNodeClass = 'listing-result__project-features'
			childNodeClass = 'listing-result__listing'
			priceNodeClass = 'listing-result__price'

		addressNode = listing.find('span', class_=addressNodeClass)
		if addressNode:
			addressParts = addressNode.text.strip().split(',')
			addressLine1 = addressParts[0].strip()
			addressLocality = addressParts[1].strip()
			try:
				addressRegion, postalCode = addressParts[2].strip().split(' ')
			except:
				print(addressNode.text)
				raise
		else:
			print(str(listing))
			raise ValueError('No project address found.')
			
		project = Project(addressLine1, addressLocality, addressRegion, postalCode)
			
		titleNode = listing.find('h2', class_=titleNodeClass)
		if titleNode:
			title = titleNode.text.strip()
			project.setTitle(title)

		urlNode = listing.find('a', class_=urlNodeClass)
		if urlNode and urlNode.has_attr('href'):
			projectUrl = urlNode['href']
			project.setUrl(projectUrl)
			print(projectUrl)
		else:
			print(str(listing))
			raise ValueError('No project URL found.')
			
		featureNode = listing.find('ul', class_=featureNodeClass)
		if featureNode:
			features = listing.find_all('li')
			for feature in features:
				project.setFeature(feature.text.strip())
				
		# get the child properties
		childListings = listing.find_all('a', class_=childNodeClass)
		if len(childListings) == 0:
			print(str(listing))
			raise ValueError('No child property found.')
		for childListing in childListings:
			print(str(childListing))
			if childListing.has_attr('href'):
				propertyUrl = childListing['href']
			else:
				print(str(childListing))
				raise ValueError('No child URL found.')
			priceNode = childListing.find('h3', class_=priceNodeClass)
			if priceNode:
				priceText = priceNode.text.strip()
				price = self.__getPrice(priceText)
			if price == 0 and addressLine1:
				price = self.__getPriceFromAddress(addressLine1, addressLocality, addressRegion, postalCode)
			if price == 0:
				print(str(childListing))
				raise ValueError('No child price found.')
			childProperty = Property(price)
			childProperty.setUrl(propertyUrl)
			
			childFeatureNodes = listing.find_all('span', class_='property-feature__feature-text-container')
			if len(childFeatureNodes) == 0:
				print(str(listing))
				raise ValueError('No child feature found.')

			for childFeatureNode in childFeatureNodes:
				childProperty.setFeature(childFeatureNode.text.strip())
				
			childPage = urlopen(propertyUrl)
			childDom = BeautifulSoup(childPage, 'html.parser')
			childAddressNode = childDom.find('button', class_='listing-details__project-title-address')
			if childAddressNode:
				print(childAddressNode.text.strip())
				childAddressParts = childAddressNode.text.strip().split('/')
				if len(childAddressParts) > 0:
					childPropertyType = childAddressParts[0].strip()
					# this is something like: Type A, Type B, Type C, Courtyard, etc.
					childProperty.setChildPropertyType(childPropertyType)
			else:
				raise ValueError('No child address found.')
				
			project.addChildProperty(childProperty)

		return project
	
	def getData(self):
		self.__logNP('URL=%s' % self.URL)
		if self.currentPage == 1:
			page = urlopen(self.URL)
			dom = BeautifulSoup(page, 'html.parser')
			self.__getSummary(dom)
			self.__getPagingInfo(dom)
			self.__getData(dom)
			self.currentPage += 1

		while self.currentPage <= self.maxPage:
			url = self.domain + re.sub(r'page=\d+$', 'page={0}'.format(self.currentPage), self.pageUrlPattern)
			page = urlopen(url)
			dom = BeautifulSoup(page, 'html.parser')
			self.__getData(dom)
			self.currentPage +=1

	def __getPagingInfo(self, dom):
		pages = dom.find_all('a', class_='paginator__page-button')
		for page in pages:
			pageNo = int(page.text)
			if pageNo > self.maxPage:
				self.maxPage = pageNo
			if page.has_attr('href'):
				self.pageUrlPattern = page['href']
			else:
				self.currentPage = pageNo

		self.__logNP("Number of pages     : %d" % self.maxPage)

	def __getSummary(self, dom):
		summary = dom.find('h1', class_='search-results__summary').text.strip()
		countObj = re.search(r'(\d+) Properties for sale in (.+),\s*(.+),\s*(\d+)$', summary)
		if countObj:
			groupCount = len(countObj.groups())
			if groupCount == 4:
				self.postalCode = countObj.group(4)
				self.addressRegion = countObj.group(3)
				self.addressLocality = countObj.group(2)
				self.propertyNum = countObj.group(1)
			elif groupCount == 3:
				self.addressRegion = countObj.group(3)
				self.addressLocality = countObj.group(2)
				self.propertyNum = countObj.group(1)
			elif groupCount == 2:
				self.addressLocality = countObj.group(2)
				self.propertyNum = countObj.group(1)
			elif groupCount == 1:
				self.propertyNum = countObj.group(1)

		self.__logNP("Area of interest    : %s" % ('{0}, {1}, {2}'.format(self.addressLocality, self.addressRegion, self.postalCode)))
		self.__logNP("Number of properties: %s" % self.propertyNum)

	def printCSVData(self):
		for property in self.propertyList:
			print(property.toCSV())

	#
	# Need to be re-implemented to save data to DB.  For now we just save it to
	# a CSV file.
	#
	def saveData(self):
		if self.name:
			fileName = "Data-%s.csv" % self.name
			writeMode = 'w'
		else:
			fileName = "Data.csv"
			writeMode = 'a'
		csvFile = open(fileName, writeMode, newline='')
		writer = csv.writer(csvFile)
		for property in self.propertyList:
			writer.writerow \
				(
					(
						property.type, property.price, property.addressLine1,
						property.addressLocality, property.addressRegion,
						property.postalCode, property.beds, property.baths,
						property.parkings, property.landArea, property.description
					)
				)
		csvFile.flush()
		csvFile.close()
		self.__logNP("Data saved to %s." % fileName)

### MAIN ###

#
# I envisioned that you have the list of 17K URLs saved in a text file.  You
# read the file and put the URLs in an array.  Here I just hard-code one URL
# in the array.
#
# The script iterates over the URL array, for each URL, grab the data and save
# them to the database.  You need to re-implement the Scraper::saveData function
# to do that.  For now I just save the data in a CSV file.
#
# I strongly recommend against rapidly hitting domain.com.au with multiple 
# threads/search requests from the same IP address.  Doing so could cause your
# requests to be flagged as a DOS attack and your IP will be either blocked or
# your requests ignored.
#
# usage: Scraper.py [-h]
#
# Scrape domain.com.au in a single-threaded fashion.
#
# optional arguments:
#   -h, --help  show this help message and exit

def main():

	parser = argparse.ArgumentParser(description='Scrape domain.com.au in a single-threaded fashion.')
	parser.parse_args()

	print("Starting - %s" % time.strftime("%m/%d/%Y %T"))
	
	# You need to populate this array with the URLs, maybe read from a file
	urls = [
		"https://www.domain.com.au/sale/kingston-qld-4114/?ptype=duplex,house,semi-detached,terrace,town-house,villa,apartment-unit-flat,block-of-units,pent-house,studio,new-house-land,new-home-designs,new-apartments&sort=price-asc",
		"https://www.domain.com.au/sale/loganlea-qld-4131/?ptype=duplex,house,semi-detached,terrace,town-house,villa,apartment-unit-flat,block-of-units,pent-house,studio,new-house-land,new-home-designs,new-apartments&amp;sort=price-asc",
		"https://www.domain.com.au/sale/runcorn-qld-4113/?ptype=duplex,house,semi-detached,terrace,town-house,villa,apartment-unit-flat,block-of-units,pent-house,studio,new-house-land,new-home-designs,new-apartments&sort=price-asc",
		"https://www.domain.com.au/sale/edens-landing-qld-4207/?ptype=duplex,house,semi-detached,terrace,town-house,villa,apartment-unit-flat,block-of-units,pent-house,studio,new-house-land,new-home-designs,new-apartments&sort=price-asc",
		"https://www.domain.com.au/sale/holmview-qld-4207/?ptype=duplex,house,semi-detached,terrace,town-house,villa,apartment-unit-flat,block-of-units,pent-house,studio,new-house-land,new-home-designs,new-apartments&sort=price-asc",
		"https://www.domain.com.au/sale/coopers-plains-qld-4108/?ptype=duplex,house,semi-detached,terrace,town-house,villa,apartment-unit-flat,block-of-units,pent-house,studio,new-house-land,new-home-designs,new-apartments&sort=price-asc",
		"https://www.domain.com.au/sale/bald-hills-qld-4036/?ptype=duplex,house,semi-detached,terrace,town-house,villa,apartment-unit-flat,block-of-units,pent-house,studio,new-house-land,new-home-designs,new-apartments&sort=price-asc",
		"https://www.domain.com.au/sale/zillmere-qld-4034/?ptype=duplex,house,semi-detached,terrace,town-house,villa,apartment-unit-flat,block-of-units,pent-house,studio,new-house-land,new-home-designs,new-apartments&sort=price-asc",
		"https://www.domain.com.au/sale/petrie-qld-4502/?ptype=duplex,house,semi-detached,terrace,town-house,villa,apartment-unit-flat,block-of-units,pent-house,studio,new-house-land,new-home-designs,new-apartments&sort=price-asc"
	]

	if os.path.exists('Data.csv'):
		os.remove('Data.csv')

	for url in urls:
		scraper = Scraper(url)
		scraper.getData()
		scraper.saveData()
		scraper = None
		
	print("Stopping - %s" % time.strftime("%m/%d/%Y %T"))

if __name__ == '__main__':
    main()
