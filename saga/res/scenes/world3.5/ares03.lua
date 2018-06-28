speak("Ares", "WHO DARES CHALLENGE ME?")

sceneSwitch('ambush_arrives', true)

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

speak("Janine", "Now let's see how tough you are without your shield!")

sceneSwitch('ares_dead', true)
battle('party_bossAres', false, 'ffl1_boss')
face('hero', 'NORTH')

speak("Janine", "The master is dead.")
speak("BORG", "I'm proud of all of you. Now let's get out of here!")

fade('white')
wait(1.0)
teleport('world3.5/fortress_b1.tmx', 20, 11, 'NORTH', false)
fade('normal')
wait(1.0)
