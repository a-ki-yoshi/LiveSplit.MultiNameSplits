# LiveSplit MultiNameSplits Technical Specification

**Last Updated**: As of 2025/06/18

## About the Fork

- **Forked from**: LiveSplit.Subsplits
- **Compatibility**: All Splits functionality is available
  - Note: Since this wasn't the case in the past, when asking AI about specifications in this area, be careful as you might get irrelevant responses
- **Starting commit**: ba1dc4b
  - https://github.com/LiveSplit/LiveSplit.Subsplits/commit/ba1dc4b90ae42fc1b038174d41ce79a0f5c1ae31
- **App compatibility**: Integrated up to the latest version at fork time (1.8.33)

## Development Requirements
- .NET Framework: 4.8.1

## Supported Versions (Tested Versions)
- 1.8.33 (latest version as of now)
- 1.8.30 (first version with .NET Framework 4.8.1 support)

## Installation Method

- Place the DLL file on GitHub's Release page for users to download
  - https://github.com/a-ki-yoshi/LiveSplit.MultiNameSplits/releases
- Otherwise, same as regular LiveSplit components

## Three Features

### [1] Multi Name Splits Feature

**Overview**: Main feature of this component
**Functionality**: Automatically switches split names at regular intervals

#### What it can do, example use cases
- Display split names alternately in native language and foreign language
- Display split names and small descriptions about segments

#### Layout Editor Settings

**Display Location**
- "Multi Split Names"
- Below "Split Names"

**Settings**

##### Separator Text
- **Default value**: "/"
- **Allowed characters**: Any characters (multiple characters allowed)
- **Restrictions**:
  - Forbidden characters: `-` `{` `}` (used by Subsplits feature)
  - Emojis: Available (not recommended)
- **Behavior**:
  - Empty string: No name switching, displays split name as-is
  - Character not in split name: Displays split name as-is
  - Whitespace and trimming:
    - If set to "/": `"  aaa  / bbb"` → `"aaa"` and `"bbb"` (trims leading/trailing spaces)
    - If set to " / ": `"aaa/bbb"` won't be separated
  - Full-width to half-width conversion:
    - Supports a~z A~Z 0~9 symbols spaces (katakana not supported)
    - "／/" input → converted to "//"
    - "aaa／bbb" with separator "/" → split into "aaa" and "bbb"
- **Error handling**:
  - Forbidden character input: Immediately auto-deleted, warning message displayed
    - Copy-pasting strings like "{/" with valid characters after forbidden ones doesn't show warning, but no current countermeasure
  - Warning message: Displayed on the right side of input field, hidden when focus is lost or edited with valid characters

##### Display Time (Seconds)
- **Description**: Time to display before switching to next name (not total time for all split names to cycle)
- **Default value**: 10
- **Range**: 1 ~ 999999 seconds
- **Unit**: Seconds
- **Operation**: Arrow buttons increment by 1 second

##### Transition Time (Seconds)
- **Description**: Effect time during transitions (from fade-in start to fade-out end)
- **Default value**: 1.0
- **Range**: 0.1 ~ 999.0 seconds
- **Unit**: Seconds
- **Example**: If set to 2 seconds, fade-out and fade-in each take 1 second
- **Operation**: Arrow buttons increment by 0.1 seconds
- **Notes**: If value is greater than 1/2 of Display Time, fade-in and fade-out timing overlaps, causing strange display (not prohibited but warning message shown)
- **Note**: "Fade Time" would be more appropriate name, but using current name due to possibility of adding transition types in the future

##### Details
- **Overview**: Function to configure display settings individually for each text part separated by delimiter characters
- **Purpose**: Used when you want to display different parts of split names with different colors, fonts, and display times
- **Notes**: 
  - Automatically calculates the maximum number of separators on initial display or when settings change, and displays that many individual setting fields
  - If the string set in Separator Text does not exist in any split name, it will not be displayed

###### Show
- **Description**: Toggle display on/off. When unchecked, the following "Display Time" and "Color and Font" settings become unavailable
- **Default Value**: true (checked)
- **Example**: If the second separator is unchecked, the second text part of each split name will be skipped in display
- **Notes**: 
  - Cannot hide all items. When only one item is checked, the checkbox becomes unclickable to prevent toggling
  - When splits have different numbers of separators, some text may not be displayed depending on which checkboxes are unchecked (no error occurs)
  - In this case, only the displayed separators will follow the Details settings

##### Display Time
- **Description**: Time to display until switching to the next name, same as "Display Time (Seconds)"
- **Default Value**: Same value as "Display Time (Seconds)"
- **Range**: 1 ~ 999999 seconds
- **Unit**: seconds
- **Operation**: Arrow buttons increment by 1 second
- **Notes**: 
  - Once set to a value different from the default, it will not sync with "Display Time (Seconds)" changes unless Reset is used

##### Color and Font (Color)
- **Description**: Can change the text color
- **Default Value**: Color set in other locations (black when settings dialog is opened. RGBA 0, 0, 0, 255)
- **Restrictions**: None
- **Notes**: 
  - When set, it overrides text color settings in "Subsplits" or "Split Names" sections
  - Once set, settings from other locations will not be reflected unless Reset is used
  - When splits have different numbers of separators, color changes abruptly without fade

##### Color and Font (Font)
- **Description**: Can change the text font
- **Default Value**: Font set in "Layout" tab (Segoe UI 16pt when settings dialog is opened)
- **Restrictions**: None
- **Notes**: 
  - If font name is too long, the name displayed on the button may be truncated
  - Once set, settings from "Layout" tab will not be reflected unless Reset is used

##### Move (Up・Down)
- **Description**: Can change the order of item settings
- **Operation**: One click changes by one line
- **Restrictions**: Up/Down buttons are disabled for topmost/bottommost items respectively
- **Effect**: Changing order does not change the display order of parts in split names

##### Reset
- **Description**: Initializes item settings and returns to default values
- **Target**: All settings for Show, Display Time, and Color and Font
- **Note**: Once Reset is used, settings changes from other locations will be reflected again

#### When different splits have different numbers of separators

**Example**: Split names are `"aaa/bbb/ccc"`, `"ddd/eee"`, and `"fff"` with separator "/"

1. "aaa" and "ddd" fade in
2. "aaa" and "ddd" fade out
3. "bbb" and "eee" fade in
4. Only "bbb" fades out. "eee" continues displaying
5. "ccc" fades in. "eee" continues displaying
6. "ccc" and "eee" both fade out
7. "aaa" and "ccc" fade in, returning to the beginning
8. During this time, "fff" continues displaying without any fade-in or fade-out

#### Other notes
- Text, shadows, outlines, and backgrounds of split names are also considered. Fade effects work smoothly even with opacity less than 255

### [2] Automatic Initial Settings Import Feature

**Overview**: 
When adding component, if "Multi Name Splits", "Subsplits", or "Splits" already exists, reflects those settings as initial settings

**Priority**:
- If multiple types exist, adopts only one in the above priority order
  - Example: If "Splits", "Subsplits", and "Multi Name Splits" already exist, uses "Multi Name Splits" settings as initial settings
  - Settings not existing in import source become default values
    - Example: If targeting "Splits" component, Subsplits-related settings don't exist, so those become default values
- If multiple components of same type exist, prioritizes the one above and adopts only one
  - Example: If "Splits", "Subsplits"(1st), "Subsplits"(2nd) already exist, uses 1st "Subsplits" settings as initial settings
- If "Subsplits" component etc. is deleted before adding component, no import occurs and default values are set
- Import source target components are only these 3 types. Others, even if forked from "Splits" etc., are not targeted
  - If we targeted others, it would be scary if something went wrong...

**Settings**: No settings related to the feature itself in Layout Editor
- Adding an import button might be more helpful, but demand seems low so won't implement in 1.0.0 at least

### [3] Subsplits ON/OFF Feature

**Overview**: When disabled, display becomes similar to Splits component (except for automatic split name switching display by feature [1])

**Purpose**: Added feature thinking some people might want only Splits functionality enabled depending on how split names are written

**Layout Editor**:
- Added "Enable Subsplits" checkbox at the top of "Subsplits" group
- **Default value**:
  - false if import source in [2] is "Splits"
  - false if import source in [2] is "Multi Name Splits" and that component's setting is false
  - true otherwise

## Secret Fixes

- Made Layout Editor easier to read by making group name labels bold and colored
- Added bottom margin to Layout Editor Columns items because they were making me anxious without it

## Troubleshooting

### Common Problems and Solutions

#### Separator doesn't work
**Cause**: Split name doesn't contain separator, or forbidden characters are used
**Solution**:
- Check if split name contains separator
- Check if forbidden characters (`-` `{` `}`) are not used
- If using full-width characters, confirm they are converted to half-width

#### Name switching doesn't display
**Cause**: Transition Time might be too long
**Solution**:
- Make Transition Time shorter (recommended: 1/2 or less of Display Time)

#### Fade-in/fade-out overlaps and display looks strange
**Cause**: Transition Time is greater than 1/2 of Display Time
**Solution**: Set Transition Time to 1/2 or less of Display Time

#### Initial settings not imported
**Cause**: Target component doesn't exist or was deleted
**Solution**:
- Check if existing "Multi Name Splits", "Subsplits", or "Splits" components exist
- If component was deleted beforehand, if layout file hasn't been saved yet:
  1. Open a different layout without saving current one
  2. Reopen the layout where you want to add the component

#### Full-width characters not correctly converted to half-width
**Cause**: Katakana is not supported by automatic conversion by specification. Others might be oversight
**Solution**: Change to half-width characters manually, and if there's possibility of oversight, please contact me

#### Don't want full-width characters converted to half-width
**Situation**: Sorry, prioritized avoiding notation variation troubles. No current solution
**Response**: If possible, please contact me with reasons why full-width is better. Contact info is in README.md

### Debugging Methods

#### Checking Debug Logs
- **For developers**: Check debug messages in Visual Studio Output window
- **Log format**: `"[MultiNameSplits] message content"`
- **Main logs**:
  - Settings loading error: `"Error getting settings: [error content]"`
  - Index error: `"Invalid SplitNameIndex: [index], Array length: [array length]"`

#### Checking Settings
- Check values of each setting item in Layout Editor
- If warning message is displayed, check its content

#### Action Verification
- Check split name division results
- Check fade-in/fade-out timing
- Check action after setting changes

#### Performance
- Memory usage may increase with large numbers of splits
- Caching feature speeds up operation after initial loading

#### Character Conversion
- Full-width to half-width conversion supports only some characters (a~z, A~Z, symbols, spaces)
- Katakana and some special characters are not converted

#### Setting Compatibility
- Complete compatibility with original Subsplits component is not guaranteed
- Some setting items have unique implementations