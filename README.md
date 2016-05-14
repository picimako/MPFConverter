# MPFConverter

This is a custom postprocessor for transforming **.MPF** to **.NCT** files.
MPF is for Siemens machines that is generated by WorkNC, meanwhile NCT is the default file extension for NCT machines.

This application includes a couple of convenient features to modify the final NCT file.

### UI

- Select the MPF file to convert.
- Set values to include or not include in the final NCT file
- Strong validation of fields not to mess up the result
- Autocopy the final file to a predefined network base location (defined in ```settings.txt```). The desired subfolder then can be browsed in that network folder.
- Multiple network configurations can be defined between which you can switch. Each saves its own settings controlled on the UI and reloads them when the network is selected again.
- Open log file of conversion.
- Autoincrement **programId** after a conversion is done

### Conversion

- Add ```%O<programId> (<Comment>)``` into the first row
- Add ```A[I]<angle>``` when Osztófej (and I) is checked on the UI. "I" is optional.
- Add ```G..Q..``` to the beginning of the file when HSHP is checked on the UI
- Make command values enclosed in brackets in rows containing the **;** character. For example:
    - Initial: ```N7;  DX=   .00000```
    - Final: ```N7(  DX=   .00000)```
- Add ```G650``` before ```M30``` (at the end of the file)
- Add a **%** character in a new row after ```M30```
- Add G..Q.. to the end of the file when HSHP is checked on the UI
    - *This is currently not working. Will be changed in a later version.*
- Look for the **T-M6-S-G** pattern in 4 consecutive rows all over the file (even multiple occurences) and apply the following change for each:
    - In the row contaning ```G0``` after the row containing ```S```, find the ```Z``` coordinate and move it to a new line as ```G43 H<the number after T> Z<value of Z>```
    - Add ```M8``` in a new row after each **T-M6-S-G** pattern

##### Notes

- ```G...Q...``` values are defined in the first row of ```settings.txt``` in **[ON]#[OFF]** pattern

# Further improvements

- Mark UI controls that contain incorrect value
