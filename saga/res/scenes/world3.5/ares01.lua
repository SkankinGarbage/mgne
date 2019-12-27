local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero4 = getHero(3).getName()

speak("David", "Where is everybody?")

pathEvent('hero', 'target_a')
pathEvent('hero', 'target_b')

speak("Grandmaster", "We're under attack!")
speak(hero1, "We have to help them.")
speak("David", "Yes! But...")
speak("David", "Waggh! It's him! " .. hero1 .. ", run!")

battle('party_bossAresInvincible', true, 'battle_boss')
face('hero', 'NORTH')

fade('white')
wait(1.0)
sceneSwitch('world2_done', true)
removeMember()
teleport('world3.5/fortress_b1.tmx', 20, 11, 'NORTH', false)
fade('normal')
wait(1.0)

speak("Grandmaster", "Is everyone safe?")
speak(hero1, "I think so.")
speak(hero2, "What was that?")
speak("David", "Ares. An invincible soldier who calls himself a 'master.'")
speak("Grandmaster", "When he attacks we can't do anything but run. He destroys everything in his path.")
speak(hero4, "Then what can we do?")
speak("David", "I don't know. We knights can't hold back the monsters anymore.")
speak("Grandmaster", "Jonas had some ideas, but he left to go defend his hometown.")

removeMember()
sceneSwitch('ares01', true)
