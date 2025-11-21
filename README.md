# BossDrops Plugin

A TShock plugin that allows you to configure bosses to drop additional items with custom quantities.

### Installation

1. Download the plugin DLL and place it in your tshock/ServerPlugins folder
2. Start the server to generate the default configuration file
3. Shutdown the server and edit the configuration file as desired

#### IMPORTANT: If you are updating the plugin, make sure to delete the old configuration file before installation, otherwise the plugin may crash trying to load the old config file structure

### Configuration

The plugin configuration is stored in `tshock/BossDrops.json` and can be edited to customize which items drop when bosses are defeated.

#### Item Format

Each boss has a list of items that will drop when defeated. The format for each item is:

```
"itemID:quantity"
```

- **itemID**: The Terraria item ID number
- **quantity**: The amount of that item to drop (optional, defaults to 1 if omitted)

#### Examples

**Basic single item:**
```json
"EoC": ["74:1"]
```

**Multiple items with different quantities:**
```json
"KS": ["4022:1", "3532:3", "499:5"]
```

**Items without specifying quantity (defaults to 1):**
```json
"EoC": ["74", "75", "76"]
```

#### Configuration File Structure

Here's a basic example of the configuration file and possible formatting:

```json
{
  "Enabled": true,
  "DropOnlyOnFirstKill": false,
  "DropItems": {
    "KS": ["4022", "3532:1", "499:69"],
    "EoC": [
      "74:12",
      "48:2",
      "54"
    ],
    "BoC": ["235:15"],
    "EoW": [],
    "QB": [],
    "Skeletron": [],
    "Deerclops": [],
    "WoF": [],
    "QS": [],
    "Destroyer": [],
    "Twins": [],
    "Prime": [],
    "Plantera": [],
    "Duke": [],
    "Empress": [],
    "Golem": [],
    "Cultist": [],
    "ML": []
  }
}
```

#### Boss Abbreviations

- **KS**: King Slime
- **EoC**: Eye of Cthulhu
- **BoC**: Brain of Cthulhu
- **EoW**: Eater of Worlds
- **QB**: Queen Bee
- **Skeletron**: Skeletron
- **Deerclops**: Deerclops
- **WoF**: Wall of Flesh
- **QS**: Queen Slime
- **Destroyer**: The Destroyer
- **Twins**: The Twins
- **Prime**: Skeletron Prime
- **Plantera**: Plantera
- **Duke**: Duke Fishron
- **Empress**: Empress of Light
- **Golem**: Golem
- **Cultist**: Lunatic Cultist
- **ML**: Moon Lord

### Commands
/bosslist
* Shows a list of bosses and whether they have been defeated

/bossreset
* Resets boss list
* Requires tshock.admin permission to execute (can also be used in console)
* The boss list is stored inside and relative to the current world file

***

###
[Download BossDrops.dll](https://github.com/onusai/tshock-boss-drops/raw/main/bin/Debug/net6.0/BossDrops.dll)