using System;
using System.Collections.Generic;

namespace UnityEngine.XR.iOS
{
	public class UnityARHitTestExample : MonoBehaviour
	{
		public Transform m_HitTransform;
		public float maxRayDistance = 30.0f;
        public LayerMask collisionLayer = 1 << 10;  //ARKitPlane layer
        public GameObject handObjPrefab;

        bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes)
        {
            Debug.Log("2222222");

            List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
            if (hitResults.Count > 0) {
                Debug.Log("333333");

                foreach (var hitResult in hitResults) {
                    Debug.Log ("Got hit!");
                    //Create a new gameobject and set as child of current object.
                    GameObject newHandObj = Instantiate(handObjPrefab);
                    newHandObj.transform.parent = this.transform;
                    newHandObj.transform.position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                    newHandObj.transform.localScale = new Vector3(1, 1, 1);
                    newHandObj.transform.rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);

                    this.GetComponent<UnityARHitTestExample>().enabled = false;
                    Debug.Log (string.Format ("x:{0:0.######} y:{1:0.######} z:{2:0.######}", m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));
                    return true;
                }
            }
            return false;
        }

        public bool IsPointerOverUI()
        {
            EventSystems.EventSystem system = EventSystems.EventSystem.current;
            if(system != null)
            {
                if(Application.isMobilePlatform)
                {
                    if(Input.touchCount > 0)
                    {
                        return system.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
                    }
                }
                else
                {
                    return system.IsPointerOverGameObject();
                }
            }
            return false;
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR   //we will only use this script on the editor side, though there is nothing that would prevent it from working on device
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                //we'll try to hit one of the plane collider gameobjects that were generated by the plugin
                //effectively similar to calling HitTest with ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent
                if (Physics.Raycast(ray, out hit, maxRayDistance, collisionLayer))
                {
                    //we're going to get the position from the contact point
                    m_HitTransform.position = hit.point;
                    Debug.Log(string.Format("x:{0:0.######} y:{1:0.######} z:{2:0.######}", m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));

                    //and the rotation from the transform of the plane collider
                    m_HitTransform.rotation = hit.transform.rotation;
                }
            }
#else

            if (IsPointerOverUI() == true)
                return;
            
			if (Input.touchCount > 0 && m_HitTransform != null)
			{
               
				var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)// || touch.phase == TouchPhase.Moved)
				{
                    Debug.Log("004");

					var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
					ARPoint point = new ARPoint {
						x = screenPosition.x,
						y = screenPosition.y
					};

                    // prioritize reults types
                    ARHitTestResultType[] resultTypes = {
						//ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingGeometry,
                        ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
                        // if you want to use infinite planes use this:
                        //ARHitTestResultType.ARHitTestResultTypeExistingPlane,
                        //ARHitTestResultType.ARHitTestResultTypeEstimatedHorizontalPlane, 
						//ARHitTestResultType.ARHitTestResultTypeEstimatedVerticalPlane, 
						//ARHitTestResultType.ARHitTestResultTypeFeaturePoint
                    }; 
					
                    foreach (ARHitTestResultType resultType in resultTypes)
                    {
                        Debug.Log("111111");
                        if (HitTestWithResultType (point, resultType))
                        {
                            return;
                        }
                    }
				}
			}
#endif

        }

	
	}
}

