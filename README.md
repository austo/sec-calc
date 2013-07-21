### Securities Position Calculator with Limited Market Simulation ###

#### Some Assumptions ####

* Profit/loss is calculated per commodity per trading book.
* The "market" initializes its prices based on the trade prices in the trades file (not especially realistic).
* Market closes when trader exits (market does not exist independently of the trader).

#### Command Line Arguments ####

* "-i" or "i" - input (trades) file
* "-o" or "o" - output (positions) file
* "-v" or "v" - market volatility as a percentage of initial price
Command line switches are not case sensitive.