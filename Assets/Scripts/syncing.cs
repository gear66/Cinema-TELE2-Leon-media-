////Attach this to the GameObject you would like to spawn (the player).
////Make sure to create a NetworkManager with an HUD component in your Scene. To do this, create a GameObject, click on it,
////and click on the Add Component button in the Inspector window.  
////From there, Go to Network>NetworkManager and Network>NetworkManagerHUD respectively.
////Assign the GameObject you would like to spawn in the NetworkManager.
////Start the server and client for this to work.

////Use this script to send and update variables between Networked GameObjects
//using UnityEngine;
//using UnityEngine.Networking;

//public class syncing : NetworkBehaviour
//{
//    public const int m_MaxHealth = 100;

//    //Detects when a health change happens and calls the appropriate function
//    [SyncVar(hook = "OnMyData")]
//    public int m_CurrentHealth = m_MaxHealth;
//    //public RectTransform healthBar;

//    public void TakeDamage(int amount)
//    {
//        if (!isServer)
//            return;

//        //Decrease the "health" of the GameObject
//        m_CurrentHealth -= amount;
//        //Make sure the health doesn't go below 0
//        if (m_CurrentHealth <= 0)
//        {
//            m_CurrentHealth = 0;
//        }
//    }

//    void Update()
//    {
//        //If the space key is pressed, decrease the GameObject's own "health"
//        if (Input.GetKey(KeyCode.Space))
//        {
//            if (isLocalPlayer)
//                CmdDataChanged(m_CurrentHealth.ToString());
//        }
//    }

//    void OnChangeHealth(int health)
//    {
//        Debug.Log("health changed " + health);
//    //healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
//}

//    //This is a Network command, so the damage is done to the relevant GameObject
//    [Command]
//    //void CmdTakeHealth()
//    void CmdDataChanged(string data)
//    {
//        //Apply damage to the GameObject
//        TakeDamage(2);
//        data = m_CurrentHealth.ToString();
//    }
//}