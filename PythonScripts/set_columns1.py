from mcje.minecraft import Minecraft
# import param_MCJE as param
import param_UNITY as param
from time import sleep

mc = Minecraft.create(port=param.PORT_MC)
mc.postToChat('set_columns1')

x = -36
z = 4
for _n in range(10):
    # y = param.Y_SEA + 1
    y = 0
    for _i in range(15):
        mc.setBlock(x, y, z,  param.IRON_BLOCK)
        sleep(0.1)
        y += 1
    x += 2

sleep(10)
mc = Minecraft.create(port=param.PORT_MC)

x = -20
z = 0
for _n in range(10):
    # y = param.Y_SEA + 1
    y = 0
    for _i in range(5):
        mc.setBlock(x, y, z,  param.IRON_BLOCK)
        sleep(0.1)
        y += 1
    x += 2
