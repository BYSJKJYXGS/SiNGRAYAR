
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Xvisio.Input.UI { 

/// <summary>
///处理自定义射线的输入
/// </summary>
[RequireComponent(typeof(EventSystem))]
    [DisallowMultipleComponent]
public class XvXRInputModule : MixedRealityInputModule
    {

      //  

    private XvInputControllerBase[] inputControllerBases;

    protected override void Awake()
    {
        base.Awake();
        inputControllerBases = FindObjectsOfType<XvInputControllerBase>();

         EventSystem.current.sendNavigationEvents = false;
    }
    public override void Process()
    {
        base.Process();//处理自带事件

        ProcessAllRaycast();
    }
    private void ProcessAllRaycast()
    {

           ///遍历所有的输入模块
        for (int i = 0; i < inputControllerBases.Length; i++)
        {
                //如果当前模块没有激活或者是不可用则不处理
            if (!inputControllerBases[i].isActiveAndEnabled) {
                continue;
            }

            if (inputControllerBases[i].Is3DInput)
            {
                    //如果是3D的射线输入，那就需要获取射线穿过的所有UI元素
                XvRaycaster customRaycaster = inputControllerBases[i].customRaycaster;
                RaycastResult result = customRaycaster.FirstRaycastResult();//获取第一个元素

                Process2DOr3DRaycast(inputControllerBases[i], result);//处理当前UI元素的逻辑


            }
            else
            {
                    //如果是屏幕上的点，则需要获取
                PointerEventData pointerEventData = new PointerEventData(eventSystem);
                  //我们需要获取自定义的屏幕输入点
                pointerEventData.position = inputControllerBases[i].screenPosition;
                    //设置按钮为LeftButton
                pointerEventData.button = PointerEventData.InputButton.Left;

                    //通过EventSystem获取射线触碰到的物体
                eventSystem.RaycastAll(pointerEventData, m_RaycastResultCache);
                RaycastResult result = FindFirstRaycast(m_RaycastResultCache);
                Process2DOr3DRaycast(inputControllerBases[i], result);
            }

        }

        // }
    }
    private void Process2DOr3DRaycast(XvInputControllerBase inputControllerBase, RaycastResult result)
    {
       
        ///获取鼠标滚轮
        var scrollDelta = inputControllerBase.GetScrollDelta();


        var customEventData = inputControllerBase.CustomEventData;
        if (customEventData == null) { return; }

        customEventData.Reset();
        customEventData.delta = Vector2.zero;
        customEventData.scrollDelta = scrollDelta;//鼠标滚动的值


        customEventData.pointerCurrentRaycast = result;//设置当前射线信息
            //两帧之间的移动位置
        customEventData.screenPositionDeltadelta = inputControllerBase.PositionDeltadelta.magnitude;

        //customEventData.position = inputControllerBase.CustomEventData.position;
        customEventData.position = inputControllerBase.screenPosition;
       



        ProcessPress(customEventData);
        ProcessMove(customEventData);
        ProcessDrag(customEventData);

        // 滚动事件
        if (result.isValid && !Mathf.Approximately(scrollDelta.sqrMagnitude, 0.0f))
        {
            var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(result.gameObject);
            ExecuteEvents.ExecuteHierarchy(scrollHandler, customEventData, ExecuteEvents.scrollHandler);
        }
        if (eventSystem.sendNavigationEvents)
        {
            SendSubmitEventToSelectedObject();
        }
        SendUpdateEventToSelectedObject();
    }
  

    #region 处理鼠按下抬起事件
    protected void ProcessPress(CustomEventData eventData)
    {
        if (eventData.GetKeyPress())
        {
            //判断是否是按下状态
            if (!eventData.pressPrecessed)
            {
                ProcessPressDown(eventData);

            }
            // Debug.LogError(eventData.eligibleForClick);
        }
        else if (eventData.pressPrecessed)
        {

            ProcessPressUp(eventData);
        }
    }
    //protected void DeselectIfSelectionChanged(GameObject currentOverGo, BaseEventData pointerEvent)
    //{
    //    // Selection tracking
    //    var selectHandlerGO = ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo);
    //    // if we have clicked something new, deselect the old thing
    //    // leave 'selection handling' up to the press event though.
    //    if (selectHandlerGO != eventSystem.currentSelectedGameObject)
    //        eventSystem.SetSelectedGameObject(null, pointerEvent);
    //}
    protected void ProcessPressDown(CustomEventData eventData)
    {
        //当前选中的物体
        var currentOverGo = eventData.pointerCurrentRaycast.gameObject;
        //设置状态为按下状态
        eventData.pressPrecessed = true;

        //设置点击状态
        eventData.eligibleForClick = true;
        //指针坐标变化
        eventData.delta = Vector2.zero;
        //设置拖拽状态
        eventData.dragging = false;
        //是否应该使用阻力阈值？
        eventData.useDragThreshold = true;
        //设置鼠标位置
        eventData.pressPosition = eventData.position;
        //设置当前按下时候的物体
        eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;
            //如果当前指向的GameObject不同，则取消选择当前选定的GameObject。
            DeselectIfSelectionChanged(currentOverGo, eventData);
        eventData.button = PointerEventData.InputButton.Left;

            //找到处理这个事件的物体并执行事件，由于IPointerDownHandler事件按下即刻出发，所以直接发送对应事件
            var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.pointerDownHandler);

            //找到IPointerClickHandler事件接收的物体，由于IPointerClickHandler事件需要在鼠标抬起的时候判断是否是同一个物体，所以这里就是获取，不执行事件
            var newClick = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
        // 
        if (newPressed == null)
        {
               
                newPressed = newClick;
        }
        //如果没有设置缩放时间，这个时间和Time.time时间相同
        var time = Time.unscaledTime;

        if (newPressed == eventData.lastPress)
        {
            //设置点击的数量
            if (time < (eventData.clickTime))
            {
                ++eventData.clickCount;
            }
            else
            {
                eventData.clickCount = 1;
            }
            //设置点击时间
            eventData.clickTime = time;
        }
        else
        {
            eventData.clickCount = 1;
        }
            //能够处理pointerDownHandler事件的物体
            eventData.pointerPress = newPressed;
            //能够处理IPointerClickHandler事件的物体，存储起来，在鼠标抬起的时候判断是否需要执行单击时间事件
            eventData.pointerClick = newClick;

        if (newPressed != null)
        {
            //Debug.LogError("newPressed==" + newPressed.name);
        }
        if (newClick != null)
        {
           // Debug.LogError("newClick==" + newClick.name);
        }
        //原始指针选中的物体
        eventData.rawPointerPress = currentOverGo;
        //
        eventData.clickTime = time;

        // 获取能够处理拖拽的物体
        eventData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

        if (eventData.pointerDrag != null)
        {
            //初始化拖拽事件
            ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.initializePotentialDrag);
        }
    }

    protected void ProcessPressUp(CustomEventData eventData)
    {

        //获取到当前物体
        var currentOverGo = eventData.pointerCurrentRaycast.gameObject;



        if (eventData.pointerPress != null)
        {
            //为按下的物体执行鼠标抬起事件
            ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);

        }
        //看看我们是否将鼠标移到单击的同一元素上。。。
        var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

        // 执行按钮点击事件
        if (eventData.pointerClick != null && eventData.pointerClick == pointerUpHandler && eventData.eligibleForClick)
        {
           ExecuteEvents.Execute(eventData.pointerClick, eventData, ExecuteEvents.pointerClickHandler);
        }
        else if (eventData.pointerDrag != null && eventData.dragging)
        {
            //如果当前鼠标抬起的时候不是同一个元素并且四拖拽状态，说明是drop事件
            ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.dropHandler);
        }
        //设置按下状态为false
        eventData.pressPrecessed = false;

        eventData.eligibleForClick = false;
        //鼠标按下时候的元素置为空
        eventData.pointerPress = null;
        eventData.rawPointerPress = null;

        //如果当前有拖动的物体，并且是拖拽状态，则发送拖动事件
        if (eventData.pointerDrag != null && eventData.dragging)
        {
            ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.endDragHandler);
        }

        eventData.dragging = false;
        eventData.pointerDrag = null;

        //重做指针进入/退出以刷新状态，
        //这样，如果我们将鼠标移到以前由于按下其他对象而忽略它的对象上，它现在就会得到它。
        if (currentOverGo != eventData.pointerEnter)
        {
            HandlePointerExitAndEnter(eventData, null);
            HandlePointerExitAndEnter(eventData, currentOverGo);
        }
    }

    #endregion
    #region 处理移动事件
    new protected void ProcessMove(PointerEventData eventData)
    {
        var hoverGO = eventData.pointerCurrentRaycast.gameObject;
        if (eventData.pointerEnter != hoverGO)
        {
            HandlePointerExitAndEnter(eventData, hoverGO);
        }

    }
    #endregion

    #region 处理拖动事件
    protected bool ShouldStartDrag(CustomEventData eventData)
    {

        bool isDrag = (eventData.screenPositionDeltadelta > 5);

      //  Debug.LogError("isDrag  ==" + isDrag + "  " + eventData.screenPositionDeltadelta);
        return isDrag;
    }

    protected void ProcessDrag(CustomEventData eventData)
    {
        eventData.button = PointerEventData.InputButton.Left;
        //Debug.LogError("拖动:" + eventData.pointerDrag != null + " " + !eventData.dragging + "  " + ShouldStartDrag(eventData));
        if (eventData.pointerDrag != null && !eventData.dragging && ShouldStartDrag(eventData))
        {
            ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.beginDragHandler);
            eventData.dragging = true;


        }




        if (eventData.dragging && eventData.pointerDrag != null)
        {
            if (eventData.pointerPress != eventData.pointerDrag)
            {
                ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);

                eventData.eligibleForClick = false;
                eventData.pointerPress = null;
                eventData.rawPointerPress = null;
            }
            ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.dragHandler);
            //Debug.LogError(eventData.pointerDrag.name);

        }
    }

    #endregion

    new protected bool SendSubmitEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return false;

        var data = GetBaseEventData();
        if (input.GetButtonDown("Submit"))
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);

        if (input.GetButtonDown("Cancel"))
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
        return data.used;
    }
    new protected bool SendUpdateEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return false;

        var data = GetBaseEventData();
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
        return data.used;
    }
}
}

