# lock-patron
A server for faking warlock patrons for use in a Neos class teaching HTTP.

## Theme of the class
The class is themed around warlocks trying to get their patron, and most of the class is around different ways to use HTTP routes to figure out what a particular user's patron is, and gaining influence with that patron. The patron may return items, instructions, new routes, and new HTTP features to the user based on how far the user has progressed in the class. The end goal is every user has favor with a patron and they are given a glyph on their avatar, that they can use as a badge for completing the class.

## What this means for the server
The server should probably be written using the same nouns and verbs as are used in the class (at least the routes are expected to respect that).

Nouns that will be important:

* **Patron**\
The first class entity that is being represented, different Patrons should essentially be considered different servers (as they will appear that way in the class). Each Patron will have a different DNS name on the end server, the DNS name will be provided probably through a header but that has yet to be decided. Some operations may be cross-Patron, for instance Patrons may be ranked and that ranking may be available via a route. The Patron list is pre-defined.

* **User**\
A transient entity that will need to be created at runtime, as part of the class a User will connect to a Patron through a client, to prove their identity the User will provide a query argument of a token provided by the server once the User introduces themselves to the Patron (which will be part of the class). At that point the User will be provided an Gift spawned from the LootTable that will be customized at runtime to contain a bearer token. That token will count as proof of authorization and will be provided in subsequent queries as proof of identity, this token will also not expire and are intentionally stealable. Once a User record is created, it is permanent and the key (the bearer token) will never expire.

* **Gift**\
A gift is a form of message sent from a User to a Patron through a POST request, or from a Patron to a User during a POST or GET request. A gift is to be sent via a `x-www-form-urlencoded` formatted payload. The `ID` field is a key into a LootTable inside Neos, all subsequent fields are parameters assigned into the newly spawned item from that LootTable. Server code may be required to represent the different behaviors, for instance LootTable item `Magic Book` may have the following parameters: `Font Name`, `Text`, `Glow Color`, `Glow Intensity`, and `Ambient Music`. Those fields are represented by slot names inside the `Magic Book` item in this case. Unmatched fields are ignored.

* **Favor**\
Favor is how liked a User is by a Patron, this may be rapidly changing based on user actions and must be synched across all instances regardless of performance. This can be thought of as a currency, and the User an account. Some gifts may cost Favor, some may provide Favor (this goes for both ends, a Patron providing an item to a user may provide Favor for them asking for it, a User providing a bad gift to a Patron may cost them favor).
