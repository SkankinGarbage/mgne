## SaGa 4

The totally not trademark-infringing etc etc

## Git setup instructions (Windows)

For the life of me I can't find where I typed all this up before so here it is again

1. Download tortoisegit (https://code.google.com/p/tortoisegit/wiki/Download)

2. Rightclick where you want to dump the source and do a Git clone...

3. For the URL enter https://github.com/psywombats/mgne.git

4. Cool you have read access now! Whenever you want, rclick somewhere in the directory and do a Git Pull to sync with the repo. We only have one branch so expect things to break all the time.

5. You now need to build the game. First off, install the JDK if you don't have it already http://www.oracle.com/technetwork/java/javase/downloads/index.html

6. Now you need to add java to your path. This is kind of annoying: https://www.java.com/en/download/help/path.xml

7. All you need to do now is run the buildscript at saga/src/build_saga.bat. If that blows up, report the error, but it should work.

8. Now game.jar should magically appear in the game directory! You can run this by double-clicking it hopefully. The .exe launcher is usually just build for releases.
