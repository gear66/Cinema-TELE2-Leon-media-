using UnityEngine;
using System.Collections;

public class rescale : MonoBehaviour {

	public GameObject Scene; // Объект, размер которого будем менять (задается в инспекторе)
	public GameObject SceneStart;
	private float timer = 10.0f; // Ставим таймер на 10 секунд
	private Vector3 scale; // Переменная для текущего размера
	private Vector3 newscale;// Переменная для нового размера
	public Vector3 velicity;
	public float t;

	// Use this for initialization
	void Start () {
		scale = new Vector3(Scene.transform.localScale.x, Scene.transform.localScale.y, Scene.transform.localScale.z); // Проверяем текущий размер
		newscale = scale;

		SceneStart.transform.position = Scene.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Rescaling (GameObject Scene) {
		//Scene.transform.localScale = Vector3.Lerp (Scene.transform.localScale, newscale, Time.deltaTime);

		//Scene.transform.rotation.Set (0, 0, 0, 0);

		//	t = Mathf.Clamp (t, 0f, 1f);
		//Scene.transform.position = Vector3.SmoothDamp (Scene.transform.position, SceneStart.transform.position,  ref velicity, 1f);
		//Scene.transform.rotation = Quaternion.Slerp(Scene.transform.rotation, SceneStart.transform.rotation, t/50f);

		Scene.transform.localScale = new Vector3 (0.97f, 0.97f, 0.97f);
		Scene.transform.rotation = Quaternion.identity;
		Scene.transform.position = new Vector3 (0.8f, 36.5f, 0.9f);
	}
}


//	void Update()
//	{
//		timer -= 1.0f * Time.deltaTime; // Отнимаем от таймера 1 в секунду
//		go.transform.localScale = Vector3.Lerp(go.transform.localScale, newscale, Time.deltaTime);
//		if (timer <= 0.0f)
//		{
//			newscale = new Vector3(Random.Range(2.0f, 10.0f), Random.Range(2.0f, 10.0f), Random.Range(2.0f, 10.0f)); // Задаем новый случайный размер
//			timer = 10.0f; // Снова ставим таймер на 10 секунд
//		}
//	}
//}
