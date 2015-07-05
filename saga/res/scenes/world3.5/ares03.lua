local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()

speak("Ares", "WHO DARES CHALLENGE ME?")

sceneSwtich('ambush_arrives', true)

walk('borg', 1, 'NORTH', false)
walk('janine', 1, 'EAST', true)
pathEvent('janine', 'janine_target', false)
pathEvent('borg', 'borg_target', true)
walk('borg', 1, 'EAST')
walk('borg', 1, 'SOUTH')
face('borg', 'EAST')
face('ares', 'WEST')
playSound('ding')
wait(0.7)

speak("Janine", "Let's see what you're made of without your shield!")

sceneSwitch('ares_dead', true)
battle('party_bossAresInvincible', false, 'ffl1_boss')

speak("Janine", "The master is dead.")
speak("BORG", "I'm proud of all of you. Now let's get out of here!")

fade('white')
wait(1.0)
teleport('world3.5/fortress_b1', 7, 11, 'NORTH', false)
fade('normal')
wait(1.0)
