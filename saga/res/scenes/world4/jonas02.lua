local hero1 = getHero(0).getName()
local hero3 = getHero(2).getName()

speak("Jonas", hero1 .. "! Any luck with EXCALIBER?")
speak(hero1, "Good to see you. We've found the two other parts!")
speak("Jonas", "Nice work. I've found a way we can get past the wastes. There's a special armor that can protect from poison.")
speak(hero3, "And you have it?")
speak("Jonas", "Not quite. But I found the location of some ruins where we should be able to find it.")
speak("Jonas", "There's a tall rock in the northeastern wastes. Let's go there first.")

pathEvent('jonas', 'hero')
addMember('chara_jonas')
sceneSwitch('jonas02', true)
