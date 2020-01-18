local hero1 = getHero(0).getName()
local hero3 = getHero(2).getName()

speak(hero1, "Jonas! Did your research pay off?")
speak("Jonas", "I found the location of a hidden lab. These places used to be all over before Moloch showed up.")
speak("Jonas", "We should be able to get the poison-resistant armor from the ruins. Then, to Moloch's lair!")
speak(hero3, "Will you show us the way?")
speak("Jonas", "There's a tall rock in the northeastern wastes. Let's go there first.")

pathEvent('jonas', 'hero')
addMember('chara_jonas')
sceneSwitch('jonas02', true)
