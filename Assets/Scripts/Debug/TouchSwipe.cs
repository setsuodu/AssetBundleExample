using UnityEngine;
using System.Collections;

public class TouchSwipe : MonoBehaviour {
	public float mScaleFactor = 2.5f;

	private Touch initialTouch = new Touch();
	private float distance = 0;
	private bool hasSwiped = false;

	void Update()
	{
		if(Input.touchCount == 2)
		{
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			Vector2 touchZeroPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePos = touchOne.position - touchOne.deltaPosition;

			float preTouchDeltaMag = (touchZeroPos - touchOnePos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			float deltaMagnitudediff = preTouchDeltaMag - touchDeltaMag;


				if(deltaMagnitudediff <= 2.5f)
				//if(Input.GetKey(KeyCode.D))
				{
					
					mScaleFactor = transform.localScale.x;
					mScaleFactor *= 1.02f;
					this.transform.localScale = new Vector3 (mScaleFactor, mScaleFactor, mScaleFactor);
				}
				if(deltaMagnitudediff > 2.5f)
				//if(Input.GetKey(KeyCode.A))
				{
					
					mScaleFactor = transform.localScale.x;
					mScaleFactor *= 0.98f;
					this.transform.localScale = new Vector3 (mScaleFactor, mScaleFactor, mScaleFactor);
				}
		}
			if(mScaleFactor >= 5f)
			{					
				this.transform.localScale = new Vector3 (5f, 5f, 5f);
			}
			if(mScaleFactor <= 1f)
			{				
				this.transform.localScale = new Vector3 (1f, 1f, 1f);
			}

	}
	
	void FixedUpdate()
	{
		if (Input.touchCount == 1)
		{
			foreach (Touch t in Input.touches) {
				if (t.phase == TouchPhase.Began) {
					initialTouch = t;
				} else if (t.phase == TouchPhase.Moved) {
					float deltaX = initialTouch.position.x - t.position.x;
					float deltaY = initialTouch.position.y - t.position.y;
					distance = Mathf.Sqrt ((deltaX * deltaX) + (deltaY * deltaY));
					bool swipedSideways = Mathf.Abs (deltaX) > Mathf.Abs (deltaY);

					if (distance > 100f) {
						if (swipedSideways && deltaX > 0) {//swiped left
							this.transform.Rotate (new Vector3 (0, 5f, 0));
						} else if (swipedSideways && deltaX <= 0) {//swiped right
							this.transform.Rotate (new Vector3 (0, -5f, 0));
						}
//					else if(!swipedSideways && deltaY > 0)//swiped down
//					{
//						this.transform.Rotate(new Vector3(0,180f,0));
//					}
//					else if(!swipedSideways && deltaY <= 0)//swiped up
//					{
//						this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, 0 ,this.GetComponent<Rigidbody>().velocity.z);
//						this.GetComponent<Rigidbody>().AddForce(new Vector3 (0,100f,0));
//					}
						hasSwiped = true;
					}
				} else if (t.phase == TouchPhase.Ended) {
					initialTouch = new Touch ();
					hasSwiped = false;
				}

			}
		}
	}

}
