WebSpeech-Talk2Dns
====================

Proof of Concept how to tunnel data trough dns. 

##Installation

1. Create a subdomain
2. Add a NS-Entry which points to your Talk2Dns-Azure-Service
3. Change the dns name in the file ´Talk2Dns.Cloud.Worker\Worker.cs´
4. Deploy to Azure

### Test
Open `nslookup` or any similar tool and set the request type to `txt`

	> set type=txt

Add a dot "." to end of the "query"-domain, in case you have a domain postfix defined.

## Supported Commands

### Time
Returns the current Time in UTC

	> time.t2d.domain.com.
	Server:  Gateway.local
	Address:  192.168.1.1
	
	Non-authoritative answer:
	time.t2d.domain.com     text =
	
	        "UTCnow: Thursday, October 30, 2014, 1:29:03 AM"	

### Weather

Get the current weather and a 5day forecast for a city

	> wtr-berlin-germany.t2d.domain.com.
	Server:  Gateway.local
	Address:  192.168.1.1
	
	Non-authoritative answer:
	wtr-berlin-germany.t2d.domain.com       text =
	
	        "Weather for: Berlin, DE
	Now: 7C, overcast clouds, 7-8C
	30.10: 8C, broken clouds, 5-11C
	31.10: 9C, scattered clouds, 6-11C
	01.11: 11C, few clouds, 8-14C
	02.11: 10C, sky is clear, 7-13C
	03.11: 11C, sky is clear, 9-14C"

### Reverse

Reverses the given parameter and returns it

	> rev-abc123.t2d.domain.com.
	Server:  Gateway.local
	Address:  192.168.1.1
	
	Non-authoritative answer:
	rev-abc123.t2d.domain.com       text =
	
	        "321cba"

### Ping / Echo
Returns the given parameter

	> ping-thepayloadishere.t2d.domain.com.
	Server:  Gateway.local
	Address:  192.168.1.1
	
	Non-authoritative answer:
	ping-thepayloadishere.t2d.domain.com    text =
	
	        "thepayloadishere"
