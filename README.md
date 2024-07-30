# tshock-boss-drops
 Configure bosses to drop additional items

***

### Setup
* Install the plugin and start tshock, this will generate the config file in `tshock/BossDrops.json`
* Close tshock, and edit the config file
* The config has a list denoted by [ ] for each boss, inside which you can put item id's (seperated by a comma if more than one)
* The list can be empty, with one, or as many item id's as you want 
* Save the config file. Note that you will always need to restart tshock after editing config file

### Commands
/bosslist
* Shows a list of bosses and whether they have been defeated

/bossreset
* Resets boss list
* Requires tshock.admin permission to execute (can also be used in console)
* The boss list is stored inside and relative to the current world file

***

[Download BossDrops.dll](https://github.com/onusai/tshock-boss-drops/raw/main/bin/Debug/net6.0/BossDrops.dll)
