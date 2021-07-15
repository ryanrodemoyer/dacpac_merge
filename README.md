# dacpac_merge

## Build Steps
1. Load the solution (`src\dacpacs.sln`) and build.
2. Open the LINQPad file (`src\dacpac_merge.linq`) and run.
3. Generate the deployment script by running the `scripts\sqlpackage_script.bat` file.
4. Open the file `scripts\merged.output.sql` to see the contents of the merged dacpacs.

## Attribution
Based on the code at https://github.com/GoEddie/DacpacMerge / https://the.agilesql.club/2019/03/how-can-we-merge-multiple-dacpacs-into-one/
