local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()

face('knight1', 'NORTH')
face('knight2', 'NORTH')
face('knight3', 'NORTH')
face('knight4', 'NORTH')

pathEvent('hero', 'target')
face('hero', 'NORTH')
wait(0.7)

playSound('explode')
shake(1.0)
wait(1.2)

speak("Grandmaster", "Why is this happening?")
speak(hero2, "What's going on?")
speak("David", "The gate is opening!")

playSound('battle')
fade('white')
wait(0.5)
sceneSwitch('shiva_appears', true)
fade('normal')
wait(0.7)

speak(hero1, "The oracle?")
speak("Oracle", "You are a fool, " .. hero1 .. ".")

wait(0.7)
face('shiva', 'EAST')
wait(0.075)
face('shiva', 'NORTH')
wait(0.075)
face('shiva', 'WEST')
wait(0.075)
face('shiva', 'SOUTH')
sceneSwitch('shiva_mutate', true)
wait(0.7)

speak("????", "Now the gate is open, and this world is mine!")

battle('party_bossShiva', false, 'battle_boss')
sceneSwitch('hero_done', true)
face('hero', 'NORTH')
