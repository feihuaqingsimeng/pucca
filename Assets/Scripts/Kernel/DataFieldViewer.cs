using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataFieldViewer : MonoBehaviour
{
    public  List<DB_Card.Schema>    List_DB_Card;
    public  List<DB_Skill.Schema>   List_DB_Skill;
    public  List<DB_Buff.Schema>    List_DB_Buff;


    void Awake()
    {
        DontDestroyOnLoad(this);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(List_DB_Card == null || DB_Card.instance != null)
            List_DB_Card = DB_Card.instance.schemaList;
        if (List_DB_Skill == null || DB_Skill.instance != null)
            List_DB_Skill = DB_Skill.instance.schemaList;
        if (List_DB_Buff == null || DB_Buff.instance != null)
            List_DB_Buff = DB_Buff.instance.schemaList;
    }
}
