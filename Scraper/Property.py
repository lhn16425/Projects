import re

class PropertyBase:

	def __init__(self, addressLine1, addressLocality, addressRegion, postalCode):
		self.addressLine1 = addressLine1
		self.addressLocality = addressLocality
		self.addressRegion = addressRegion
		self.postalCode = postalCode
		self.description = ''
		self.type = ''
		self.landArea = 0
		self.url = ''

	def setUrl(self, url):
		self.url = url
		
	def setType(self, type):
		self.type = type
		
	def getType(self):
		return self.type
		
	def setLandArea(self, landArea):
		self.landArea = landArea
		
	def setDescription(self, description):
		self.description = description

class Property (PropertyBase):

	def __init__(self, price, addressLine1 = '', addressLocality = '', addressRegion = '', postalCode = ''):
		PropertyBase.__init__(self, addressLine1, addressLocality, addressRegion, postalCode)
		self.price = price
		self.childPropertyType = ''
		self.beds = 0
		self.baths = 0
		self.parkings = 0
		
	def setChildPropertyType(self, childPropertyType):
		self.childPropertyType = childPropertyType

	def setFeature(self, feature):
		feature = re.sub(r'−', '', feature).strip()
		try:
			count, what = feature.split(' ')
		except:
			print(feature)
			raise
		if re.match(r'Beds?', what):
			self.beds = int(count)
		elif re.match(r'Baths?', what):
			self.baths = int(count)
		elif re.match(r'Parkings?', what):
			self.parkings = int(count)
		else:
			raise ValueError('Unrecognized property feature: %s' % what)
		
	def toCSV(self):
		return \
			'"{0}",{1},"{2}","{3}","{4}",{5},{6},{7},{8},{9},"{10}"'.format \
				(
					self.type, self.price, self.addressLine1, self.addressLocality,
					self.addressRegion, self.postalCode, self.beds, self.baths,
					self.parkings, self.landArea, self.description
				)
		
class Project (PropertyBase):

	def __init__(self, addressLine1, addressLocality, addressRegion, postalCode):
		PropertyBase.__init__(self, addressLine1, addressLocality, addressRegion, postalCode)
		self.title = ''
		self.featureList = []
		self.propertyList = []
		
	def setTitle(self, title):
		self.title = title

	def setFeature(self, feature):
		self.featureList.append(feature)
		
	def addChildProperty(self, property):
		self.propertyList.append(property)
		
	def getChildPropertyCount(self):
		return len(self.propertyList)
		
	def toCSV(self):
		first = True
		csvData = ''
		for property in self.propertyList:
			if first:
				propertyDelim = ''
				first = False
			else:
				propertyDelim = '\n'
			csvData += '{}"{}","{}","{}","{}","{}",{},{},{}'.format( \
				propertyDelim, property.price, "%s / %s" % (property.childPropertyType, \
				self.addressLine1), self.addressLocality, self.addressRegion, \
				self.postalCode, property.beds, property.baths, property.parkings)
		return csvData