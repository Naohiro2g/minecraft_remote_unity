from mcje.minecraft import Minecraft
# import param_MCJE as param
import param_UNITY as param
from time import sleep

mc = Minecraft.create(port=param.PORT_MC)


def setPyramid(mc=mc, x=0, z=0, size=3, y=param.Y_SEA + 1, blockTypeId=param.GOLD_BLOCK):
    while size > 0:
        mc.setBlocks(x, y, z, x + size - 1, y, z + size - 1, blockTypeId)
        x += 1
        z += 1
        size -= 2
        y += 1
        sleep(0.2)


def clearField(mc=mc):
    """ remove blocks to make space """
    mc.setBlocks(-50, param.Y_SEA + 1, -50, 50, param.Y_SEA + 20, 50, param.AIR)


if __name__ == "__main__":
    # mc = Minecraft.create(port=param.PORT_MC)
    mc.postToChat('set_pyramid1')

    # clearField()
    sleep(1)
    # setPyramid(x=10, z=-20, y=param.Y_SEA + 1, size=21)
    setPyramid(x=20, z=-20, y=0, size=51)
    sleep(2)
    setPyramid(x=-25, z=-45, y=0, size=37, blockTypeId=param.IRON_BLOCK)
