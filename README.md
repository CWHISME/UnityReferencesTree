# UnityReferencesTree
用于以树形结构显示Unity3D中，指定物体的所有引用层级关系(编辑器工具)

# 使用

对着需要查找引用的物体，点击鼠标右键->FindRefernces即可。
如图:

![右键菜单](http://7xp0w0.com1.z0.glb.clouddn.com/%5B2016.11.17%5DRightClik.png)

除此之外，在菜单上的Tools->Open Refernces Find Window也可以直接打开该界面...就是这样。

我放在Github上的场景，添加了一个示例，右键Example中的Object文件，便可以查看到如下引用：

![引用示例](http://7xp0w0.com1.z0.glb.clouddn.com/%5B2016.11.17%5Dexample.png)

另外，菜单上还有几个选项，比如查找模式和显示模式等。
显示模式主要影响显示排版，而查找模式....则是因为刚开始误会了主程的意思，多做出来的一个功能：显示引用文件的目录结构。

因为考虑到有时候没准也有用，所以就留着了。

更多详情可查看项目Example。