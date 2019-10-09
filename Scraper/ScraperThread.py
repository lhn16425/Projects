import sys
import threading
import time
import argparse
from Scraper import Scraper

class ScraperThread (threading.Thread):
	def __init__(self, threadID, url, lock):
		threading.Thread.__init__(self)
		self.threadID = threadID
		self.name = "Thread-%d" % threadID
		self.url = url
		self.lock = lock

	def run(self):
		print("%s: starting - %s" % (self.name, time.strftime("%m/%d/%Y %T")))
		scraper = Scraper(self.url, self.name)
		scraper.getData()
		self.lock.acquire()
		scraper.saveData()
		self.lock.release()
		print("%s: stopping - %s" % (self.name, time.strftime("%m/%d/%Y %T")))
	  
### MAIN ###

#
# usage: ScraperThread.py [-h] [-t N] [-d S]
#
# Scrape domain.com.au in a multi-threaded fashion.
#
# optional arguments:
#   -h, --help  show this help message and exit
#   -t N        set maximum number of scrape threads that can run in parallel,
#               default = 4
#   -d S        set number of seconds to wait before attempting to start another
#               thread, default = 2
#

def main():

	parser = argparse.ArgumentParser(description='Scrape domain.com.au in a multi-threaded fashion.')
	parser.add_argument('-t', dest='maxScrapeThreads', help='set maximum number of scrape threads that can run in parallel, default = 4', default=4, metavar='N', type=int)
	parser.add_argument('-d', dest='delay', help='set number of seconds to wait before attempting to start another thread, default = 2', default=2, metavar='S', type=int)
	args = parser.parse_args()

	MAX_SCRAPE_THREADS = args.maxScrapeThreads	# max number of scrape threads (not counting the main thread)
	DELAY = args.delay							# when we run out of scrape threads, this is the number of seconds to wait until attempting to start another one

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
	
	print("MAIN: starting - %s, MAX_SCRAPE_THREADS=%d, DELAY=%d (s)" % (time.strftime("%m/%d/%Y %T"), MAX_SCRAPE_THREADS, DELAY))
	
	lock = threading.Lock()
	
	threadID = 1
	while len(urls) > 0:
		if threading.activeCount() >= (MAX_SCRAPE_THREADS + 1):
			time.sleep(DELAY)
		else:
			url = urls.pop()
			scraperThread = ScraperThread(threadID, url, lock)
			scraperThread.start()
			threadID += 1
			
	# wait for all the search threads to finish
	while threading.activeCount() > 1:
		time.sleep(DELAY)

	print("MAIN: stopping - %s" % time.strftime("%m/%d/%Y %T"))

if __name__ == '__main__':
    main()