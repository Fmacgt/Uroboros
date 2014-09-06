
using UnityEngine;
using System.Collections;

using System;
using System.IO;

// to load and set 'song-specific' config
// also coordinates the initialization of the game (should be segregated)
public class ConfigLoader : MonoBehaviour
{
	protected struct LevelConfig
	{
		public Spawner.Config spawnerConfig;
		public FreqHold.Config freqHoldConfig;
	}

	public string songIdxKey = "SongIndex";
	public GameObject[] songList;

	public Spawner spawner;
	public FreqHold freqHold;
	public Progress progressRing;
	public GameOverUI gameOverUI;

	public int songIdx;
	private LevelConfig[] configList;
	private AudioSource song;

	void Awake()
	{
		// instantiate the corresponding audio source
		//GameObject songObj = (GameObject)Instantiate(songList[songIdx]);
		GameObject songObj = GameObject.Find("Menu Camera");

		song = songObj.GetComponent<AudioSource>();

		_loadAllConfig();
	}

	void Start()
	{
		// read data from the config file and use them to config the objects
		initConfig();
	}

	public void initConfig()
	{
		//get the index of the song to play
		songIdx = PlayerPrefs.GetInt(songIdxKey);

		spawner.setConfig(configList[songIdx].spawnerConfig);
		
		configList[songIdx].freqHoldConfig.src = song;
		freqHold.setConfig(configList[songIdx].freqHoldConfig);
		
		progressRing.setLength(song.clip.length);
		progressRing.src = song;
		
		gameOverUI.setToDistort(song);
		
		song.Play();
		freqHold.activate();
	}
	///////////////////////////////////////////////////////////////////////////
	
	private void _loadAllConfig()
	{
		StreamReader reader = new StreamReader(
				new FileStream("config.dat", FileMode.Open));

		// for now assume reader is always valid
		int songNum = Convert.ToInt32(reader.ReadLine());
		configList = new LevelConfig[songNum];
		for (int i = 0; i < songNum; i++) {
			// read Spawner config
			configList[i].spawnerConfig = new Spawner.Config();
			configList[i].spawnerConfig.diffCoeff = Convert.ToSingle(reader.ReadLine());
			configList[i].spawnerConfig.platformRatio = Convert.ToSingle(reader.ReadLine());

			// read FreqHold config
			configList[i].freqHoldConfig = new FreqHold.Config();
			configList[i].freqHoldConfig.channelNum = Convert.ToInt32(reader.ReadLine());

			configList[i].freqHoldConfig.sampleNum = Convert.ToInt32(reader.ReadLine());
			configList[i].freqHoldConfig.samplePts = 
				new int[configList[i].freqHoldConfig.sampleNum];
			for (int j = 0; j < configList[i].freqHoldConfig.sampleNum; j++) {
				configList[i].freqHoldConfig.samplePts[j] =
					Convert.ToInt32(reader.ReadLine());
			}

			configList[i].freqHoldConfig.weightNum = Convert.ToInt32(reader.ReadLine());
			configList[i].freqHoldConfig.weights = 
				new float[configList[i].freqHoldConfig.weightNum];
			for (int j = 0; j < configList[i].freqHoldConfig.weightNum; j++) {
				configList[i].freqHoldConfig.weights[j] =
					Convert.ToSingle(reader.ReadLine());
			}

			configList[i].freqHoldConfig.delay = Convert.ToSingle(reader.ReadLine());
		}

		reader.Close();
	}
	
	private Spawner.Config _loadSpawnerConfig()
	{
		return new Spawner.Config();
	}

	private FreqHold.Config _loadFreqHoldConfig()
	{
		return new FreqHold.Config();
	}
}
