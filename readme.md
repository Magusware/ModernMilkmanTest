# Modern Milkman - API Test

## How long this took
Overall I think I spent about 10-12 hours on the code, maybe more. I wanted to make a good impression and demonstrate as much of what I can do as possible. 

## Things I would do differently
In an actual full production implementation I would ensure every testable part of the code was unit tested. In this submission there are areas not covered by unit tests as I feel I demonstrated my understanding and ability to write unit tests.

The CustomerService would exist as an actual Service rather than a ClassLibrary. For this test I thought it would be overkill to implement something like ServiceFabric and it would have complicated being able to demonstrate the code due to 3rd-party dependencies.

I would have actual database integration and not the InMemory entity framework database which is intended for testing purposes only. I chose to use the InMemory database so I could spend more time on other areas as well as allowing this code to be easily run without any messing around of databases. The InMemory database, however, does not adhere to constraints and will straight up ignore them, so my configuration in the DbContext is pointless. That said, I wanted to demonstrate that I am aware and capable of building in constraints and relation between objects.

I would implement some logging utilities so any exceptions are logged.
The final thing I would do differently would be around validation of the customer and address objects. Currently it is a rather crude implementation of validation, however, given more time, 
I would take an approach which utilises attributes on the model properties. Doing this would allow for new properties to be added to the models without the need of changing any validation code.