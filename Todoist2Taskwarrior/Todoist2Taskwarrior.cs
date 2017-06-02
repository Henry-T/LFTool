using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

public static class Program
{
	public static int Main(String[] argv)
	{
		// target command 
		// task add content_str tag project annotation create_time due is_complete priority is_deleted
		// 
		JObject jObj = JObject.Parse(File.ReadAllText(argv[0]));

		List<String> resultCommands = new List<String>();

		JArray projects = (JArray)jObj["Projecs"];
		JArray items_ = (JArray)jObj["Items"];
		var labels = jObj["Labels"];


		var items = (from i in items_
			where (int)i["checked"]==0
			select new {
				Description = i["content"],
				projects = (from p in projects
					 where ((int)p["id"]==(i["project_id"]==null?0:(int)i["project_id"]))
					 select (p!=null)?(string)p["name"]:""),
				labels = jObj["Labels"].Where(l=>i["labels"].Contains(l["id"])).ToArray(),
				// Status = ((int)i["is_deleted"]==1?"Deleted":((int)i["checked"]==1?"Completed":"Pending")),
				// // Entered = convertDateString(i["date_added"]),
				// // Due = convertDateString(i["due_date"]),
				Priority = convertPriority((int)i["priority"]),
			}).ToArray();
		;


		String resultStr = "";
		foreach(var i in items)
		{
			string tagStr = "";
			foreach(var t in i.labels)
			{
				tagStr += " +"+t+" ";
			}

			string priorityStr = "";
			if (i.Priority != null)
				priorityStr = " priority:"+i.Priority+" ";

			string projectStr = "";
			foreach(string pName in i.projects)
				projectStr = " project:"+pName;

			string cmdStr = "task add "
				+i.Description 
				// +projectStr
				+tagStr
				+priorityStr;


			resultStr += cmdStr+"\n";
		}

		File.WriteAllText(argv[1], resultStr);

		return 0;
	}

	public static string convertPriority(int p)
	{
		if(p == 2)
			return "H";
		else if (p == 3)
			return "M";
		else if (p== 4)
			return "L";
		else
			return null;
	}

}