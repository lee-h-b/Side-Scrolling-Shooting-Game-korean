//파일을 읽고 다시 그 파일을 쓸때는 기존거를 닫아야한다, 그게 다른함수여도말이지!
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

//업적타입종류 플레이,적사살,돈,총발사횟수,이동거리
public enum ACHIEVEMENTTYPE { PLAY, ENEMY, MONEY, BULLET, MOVE };

[System.Serializable]
public struct Achievement//이거 db화시키면 좋을듯
{
    public string name;//이름
    public string Description;//설명
    public int needPoint;//필요한 점수정도
    public ACHIEVEMENTTYPE needType;
    //      public int id;
    public Achievement(string _name = "", string _description = "", int _needPoint = 0, ACHIEVEMENTTYPE _type = 0)
    {
        //          id = _id;
        name = _name;
        Description = _description;
        needPoint = _needPoint;
        needType = _type;
        //달성하면 해당 id에다가 true로 바꿔야함
    }
}


public class AchievementManager : MonoBehaviour {
    static public AchievementManager instance;

    public struct CountList
    {
        public int killEnemy;
        public int spentMoney;
        public int distance;
        public int useBullet;
        public int playCount;
    }
    [SerializeField]
    private TextAsset achievements;//텍스트 에셋은 수정이 불가하다..
                                   //    public TextAsset textfile;//업적들

    //외부접근 용이를위해
    public CountList record;//외부에서 쓸거같아서 public
    public GameObject alarmObject;
    private string recordPath;//카운트할것들을 담는쪽에가깝
    public List<int> clearList = new List<int>();

    public Dictionary<int,Achievement> Achievements = new Dictionary<int, Achievement>();
    ///////////    /// 
    public void Alarm(string clearname)
    {
        var temp =Instantiate(alarmObject,GameObject.Find("Canvas").transform);
        temp.transform.Find("AchieveName").GetComponent<Text>().text = clearname;
        
        //애니가 있을경우에만
        if(temp.GetComponent<Animation>() != null)
        {
            temp.GetComponent<Animation>().Play(temp.GetComponent<Animation>().clip.name);
            Destroy(temp, temp.GetComponent<Animation>().clip.length);//애니클립 길이의 시간후 소멸
        }
    }
    private void ReadAchievements()
    {
        
        StringReader reader = new StringReader(achievements.text);
        string data = reader.ReadLine();
        while(data != null)
        {
            string[] line = data.Split(',');

            //값없으면브레이크
            if (line[0] == "")
            {
                data = reader.ReadLine();
                continue;
            }
            int id = int.Parse(line[0]);

            string name = line[1];
            string description = line[2];
            int needPoint = int.Parse(line[3]);
            ACHIEVEMENTTYPE type = (ACHIEVEMENTTYPE)int.Parse(line[4]);
            //위의 5개를 통해서 1줄읽고 추가하고를 반복
            Achievements.Add(id, new Achievement(name, description, needPoint, type));
           data= reader.ReadLine();
        }
        reader.Close();
//        TextAsset record = recordPath;
    }
    private void ReadRecord()
    {
        StreamReader reader = new StreamReader(recordPath);
        string name;
        string data = reader.ReadLine();
        int value = 0;
        while(data != null)
        {
            string[] line = data.Split('=');//밸류값을 =형태로 구분해서잡을것
            name = line[0];

            if(line.Length != 1)//길이체크를 통해 클리어업적 목록s에선 추가가 안되게함
            value = int.Parse(line[1]);
            switch(name)//커에따라 다른 픽
            {
                //    public enum ACHIEVEMENTTYPE { PLAY,ENEMY,MONEY,BULLET,MOVE};
                //타입에 따른형태로 해서 가독성노림
                case "PLAY":
                case "PLAY ":
                    record.playCount = value;
                    break;
                case "KILL":
                case "KILL ":
                    record.killEnemy = value;
                    break;
                case "SPENTMONEY":
                case "SPENTMONEY ":
                    record.spentMoney = value;
                    break;
                case "DISTANCE":
                case "DISTANCE ":
                    record.distance = value;
                    break;
                case "SPENTBULLET":
                case "SPENTBULLET ":
                    record.useBullet = value;
                    break;
                default:
                    //name이 어떤값도안될경우는 //line0을 인트로간주,추가 but 뭔가있어야함
                    if(line[0] != "")
                    clearList.Add(int.Parse(line[0]) );
                    break;
                    //케이스별로
            }
            data = reader.ReadLine();
        }
        reader.Close();
    }
    private void MakeRecord()
    {
        FileStream file = new FileStream(recordPath, FileMode.Append);
        StreamWriter writer = new StreamWriter(file);
        writer.WriteLine("PLAY = 0");
        writer.WriteLine("KILL = 0");
        writer.WriteLine("SPENTMONEY = 0");
        writer.WriteLine("DISTANCE = 0");
        writer.WriteLine("SPENTBULLET = 0");
        
        writer.Close();
    }
    //아이디를 매개로 받아서 해당 아이디의 업적을 클리어로 값변경
    //그렇다면 값을 추가하는것말고도 해당 계열이 클리어인가 확인하는애도 만들어야함
    private void AchievementClear(int ID)//이아디의 값이 클리어 됐다는것
    {
        //읽기담당은 없어도됨 그냥 단순하게 추가하기때문ㅇ
        
        FileStream stream = new FileStream(recordPath, FileMode.Append);
        StreamWriter writer = new StreamWriter(stream);
            //이미참이면 브레이크 << 이건 실행전에 체크해넣어서 보낼거같다

            //이거 제거대상왜냐면 체크가함
            /*            if (clearList.Contains(ID))
                        {
                            break;
                        }
                        //아이디가 같지않으면컨티뉴
                        if (int.Parse(texts[0]) != ID)
                            continue;

                        else//없을경우 
              */
                clearList.Add(ID);//아이디를 넣어주고
                writer.WriteLine(ID);//레코드아래에도 하나 써줄것
        Alarm(Achievements[ID].name);//알람쑈함해야지!
        writer.Close();
        stream.Close();
    }
    //타입이 같을경우에만 계속하고 다르면 멈춤
    private void CheckAchievement(ACHIEVEMENTTYPE type)//이 타입으로해서 이게 클리어된건지 확인 값은 안받음 레코드를 뒤집어볼꺼임
    {
//        StreamReader reader = new StreamReader(achievePath);

        int typeVal = 0;//해당 타입에 있는 값정도

        //비교할값 대입
        switch(type)
        {
            case ACHIEVEMENTTYPE.PLAY:
                typeVal = record.playCount;
                break;
            case ACHIEVEMENTTYPE.ENEMY:
                typeVal = record.killEnemy;
                break;
            case ACHIEVEMENTTYPE.MONEY:
                typeVal = record.spentMoney;
                break;
            case ACHIEVEMENTTYPE.BULLET:
                typeVal = record.useBullet;
                break;
            case ACHIEVEMENTTYPE.MOVE:
                typeVal = record.distance;
                break;
        }
        foreach(KeyValuePair<int,Achievement> cur in Achievements)
        {
            if (clearList.Contains(cur.Key))//만약 클리어 리스트에 키가 이미있다면
                continue;//넘어감

            //타입이 같으면서 요구치에 충족할경우
            if (cur.Value.needType == type && cur.Value.needPoint <= typeVal)
            {
                AchievementClear(cur.Key);//해당키를 클리어했다는뜻
            }
        }
    }

    //타입 뒤에 밸류인 이유는 돈은 +1로 해결이 안됨
    public void RecordValueAdder(ACHIEVEMENTTYPE type = ACHIEVEMENTTYPE.PLAY,int val = 1)
    {
        StreamReader reader = new StreamReader(recordPath);
        string texts = reader.ReadToEnd();//끝까지 읽음 이게 실제로 비교대상이아닐것
        string newText;//새로 덮어씌울 텍스트

        string textcopy = "";//찾아야할 텍스트임
        string replaceText = "";//변동된 값이 들어간 텍스트

        switch (type)
        {
            case ACHIEVEMENTTYPE.PLAY:
                //이거 좀이상하긴 한데 이렇게 해야 값변경이 용의(?)할거같음
                textcopy = "PLAY = " + record.playCount.ToString();
                record.playCount += val;
                replaceText = "PLAY = " + record.playCount.ToString();
                break;
            case ACHIEVEMENTTYPE.ENEMY:
                textcopy = "KILL = "  + record.killEnemy.ToString();
                record.killEnemy += val;
                replaceText = "KILL = " + record.killEnemy.ToString(); 
                break;
            case ACHIEVEMENTTYPE.MONEY:
                textcopy = "SPENTMONEY = " + record.spentMoney.ToString();
                record.spentMoney += val;
                replaceText = "SPENTMONEY = " + record.spentMoney.ToString();
                break;
            case ACHIEVEMENTTYPE.BULLET:
                textcopy = "SPENTBULLET = " + record.useBullet.ToString();
                record.useBullet += val;
                replaceText = "SPENTBULLET = " + record.useBullet.ToString();
                break;
            case ACHIEVEMENTTYPE.MOVE:
                textcopy = "DISTANCE = " + record.distance.ToString();
                record.distance += val;
                replaceText = "DISTANCE = " + record.distance.ToString();
                break;
        }
        if (textcopy == "")
            return;//값이없으면 걍나감
                   //이제 이값을 기록에 넣어줄차례
        reader.Close();//읽기는끝났음

        //파일스트림 설정, 이걸 열어서 값을 수정함 < append가 아니라 아랫줄에 안쓰게됨
        FileStream file = new FileStream(recordPath, FileMode.Open);        
        StreamWriter writer = new StreamWriter(file);
        //1줄을 바꾸는식과 값만 바꾸도록 노력해보기 2개가 있겠는데
        //후자로해봤자 값대입에서 전자와 비슷할거같음
        //새 텍스트안에서 텍스트 카피를 리플레이스 텍스트로 변경해라
        newText = texts.Replace(textcopy, replaceText);
        writer.Write(newText);

        //순서는 크게 상관없겠지만 다쓴뒤에 깬없적이 있나 함봄
        writer.Close();

        CheckAchievement(type);



    }
    private void Start()
    {
        
        if (instance == null) instance = this;
        DontDestroyOnLoad(this);
        recordPath = Application.dataPath;
        recordPath += "/Resources/Record.txt";
//        achievePath = Application.dataPath + "/Resources/Achievement.txt";
        //만약 기록이 이미 있을경우


        // test();
        ReadAchievements();
        //이미존재
        if (System.IO.File.Exists(recordPath))
        {
            ReadRecord();
        }
        else
        {
            System.IO.File.WriteAllText(recordPath,"");//전부쓰고만들기
            MakeRecord();
            //기록이 없을경우
        }
    }
    //업적들을 담는역활임
/*    public void test()
    {
        StreamReader reader = new StreamReader(achievePath);
        string data = reader.ReadLine();
        while (data != null)
        {
            string[] line = data.Split(',');
            int id;
            string[] text = new string[2];
            int needPoint;
            
            //여쯤에 조건해야하지만 일단은 안함
            id = int.Parse(line[0]);
            text[0] = line[1];
            text[1] = line[2];

            data = reader.ReadLine();
                        
        }
        reader.Close();
    }*/

	// Update is called once per frame
}
