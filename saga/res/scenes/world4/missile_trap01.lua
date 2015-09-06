walk('guard_l1', 'WEST', 1, false)
walk('guard_r1', 'EAST', 1)
face('guard_l1', 'NORTH')
face('guard_r1', 'NORTH')

sceneSwitch('missile_trap01_01', true)
shake(0.5, 4)
wait(0.5)
sceneSwitch('missile_trap01_02', true)
shake(0.5, 4)
wait(0.5)

sceneSwitch('missile_trap01', true)
