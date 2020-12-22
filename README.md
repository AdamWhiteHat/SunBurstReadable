# SunBurstReadable
SunBurst source code. SunBurst is the SolarWinds malware.

This code was taken from [https://github.com/etlownoise/fakesunburst](https://github.com/etlownoise/fakesunburst). However, you'll notice all the code exists in a single 5000+ line code file.
In order to make the code more readable and digestible, I broke up the code, placing each class into its own .cs file and then fixed any errors. Additionally, I placed all the shared settings in it's own class, and all the shared utility functions in it's own class.

This made the code a lot more digestible and understanding the code was a breeze after that. This version of the SunBurst source code is designed to be much more readable, hence the name, SunBurstReadable.

 * You may notice that the project type is currently a .NET library. This means it is not executable, and the Main function in the Program class is not an actual entry point.
This is to prevent accidental execution, as well as, perhaps, intentional execution; This code cannot be taken and used as malware without modification.
 * The code was no longer able to be tested/executed anyways; windows now prevents this. Even fragments of the code will not run, based on heuristics (I personally encountered this trying to decrypt some strings).
 * This version has also been defanged in several places throughout the code to prevent the harmful areas of the code from being hit.
 * The C&C domain names and IP addresses have been sinkholed by Microsoft and will no longer work.
 * I have made other subtle changes to frustrate efforts of someone attempting to use certain parts of this code for malicious purposes.
