using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FakeVideoData : MonoBehaviour {

    public GameObject[] viewCounts;
    public GameObject[] dayCounts;

    bool ok = false;

    public string date;
    public int day;
    public int dayPassed;
    public int month;
    public int newViewCount;
    public DateTime startDate;

    // Use this for initialization
    void Start () {
        startDate = new DateTime(2018, 6, 10, 0, 0, 0);
        day = DateTime.Today.Day;
        month = DateTime.Today.Month;
        //dayPassed = DateTime.Compare(DateTime.Now, startDate);
        dayPassed = (int)(DateTime.Now - startDate).TotalDays;
    }
	
	// Update is called once per frame
	void Update () {
        if (!ok)
        {
            date = System.DateTime.Today.ToShortDateString();
            Debug.Log("Date is: " + date);
            foreach (GameObject b in dayCounts)
            {
                b.GetComponent<TextMesh>().text = (dayPassed.ToString() + " дней назад");
            }
            for (int i = 0; i < viewCounts.Length; i++)
            {
                if (i == 3 | i == 4)
                {
                    SetNewViewCount(i + 27);
                    viewCounts[i].GetComponent<TextMesh>().text = (newViewCount + " просмотров");
                }
                else if(i == 1 | i == 5 | i == 8)
                {
                    SetNewViewCount(i + 2);
                    viewCounts[i].GetComponent<TextMesh>().text = (newViewCount + " просмотров");
                }
                else if (i == 6 | i == 9)
                {
                    SetNewViewCount(i - 132);
                    viewCounts[i].GetComponent<TextMesh>().text = (newViewCount + " просмотров");
                }
                else
                {
                    SetNewViewCount(i - 54);
                    viewCounts[i].GetComponent<TextMesh>().text = (newViewCount + " просмотров");
                }
            }
            ok = true;
        }
    }

    public void SetNewViewCount(int a)
    {
        newViewCount = (month * 21) + (day * 14) + (int)(DateTime.Now - startDate).TotalHours + a;
    }
}
