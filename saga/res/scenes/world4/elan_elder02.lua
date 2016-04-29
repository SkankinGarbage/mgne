local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()

speak("Seer", "So you're the party that rescued Itamor. He tells me you're looking for parts of EXCALIBUR.")
speak(hero1, "We're trying to find the mystic swords before the masters can use them.")
speak("Seer", "I happen to own a shard, but we can't reforge it without the rest.")
speak(hero2, "We'll find the other parts. Trust us!")
speak("Seer", "Take the piece then, and when you have the rest, come back to the forge. Don't fail me.")

sceneSwitch('elan_elder02', true)
