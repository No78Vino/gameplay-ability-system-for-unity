# Character Controller重力插件使用说明
## 简介
Unity的Character Controller组件和rigidbody的重力位移是冲突的。所以Character Controller组件的重力需要额外实现。
这个插件实现了一个简易的重力控制。

## GravityForCharacterController 接口说明
- Instance 插件实例:懒加载，初始化会自动创建更新的Host GameObject
- Enable() 启用重力控制
- Disable() 禁用重力控制
- SetGravity(float gravity) 设置重力值
- GetGravity() 获取当前重力值
- SetGroundDetectionMethod(GroundDetectionMethod method) 设置地面检测方法
  -  GroundDetectionMethod.Default:  使用CharacterController默认的地面检测方法isGrounded
  - GroundDetectionMethod.SphereCheck: 使用SphereCheck方法检测地面.在CharacterController底部画一个小球检测范围
- SetGroundDistance(float distance) 设置地面检测用的小球半径
- SetGroundMask(LayerMask layer) 设置地面检测的LayerMask
- Register(CharacterController controller, float rate = 1f) 注册启用重力的CharacterController组件
  - rate: 单个CharacterController的重力强度，默认1.0 
  - 可以通过注册时重力强度的控制，实现个体之间的重力差异
- Unregister(CharacterController controller) 注销CharacterController组件

> 接口设计非常简单，也没怎么考虑性能优化。适合测试和小项目使用。