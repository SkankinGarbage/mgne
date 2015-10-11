local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()

speak("???", "Who goes there?")
speak(hero1, "I'm " .. hero1 .. ".")
speak(hero2, "We come from Babel to stop the masters.")
speak("???", "Interesting. Maybe I can help you out.")
speak("???", "I can make lost magic from $UFIRE, $UWATER, $UAIR, and $EARTH. Bring me one of each first and then we'll see.")

sceneSwitch('mana01', true)
