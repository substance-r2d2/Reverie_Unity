using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public sealed class SoundManager
{
	private static readonly SoundManager m_instance = new SoundManager();
	public bool enableSound = true;
	public bool enableMusic = true;

	private List<GameObject> m_ptrSounds;
	
	private SoundManager()
	{
		m_ptrSounds = new List<GameObject>();
	}
	
	public static SoundManager SharedInstance
	{
		get
		{
			return m_instance;
		}
	}
	
	// TODO URGENT: Add support for loop count
	public AudioSource PlayClip(string fileName, float volume = 1.0f, bool loop = false)
	{
		if (enableSound) 
		{
			GameObject sound = new GameObject ("Audio: " + fileName);
			GameObject.DontDestroyOnLoad (sound);
			AudioSource source = sound.AddComponent<AudioSource> ();
		
			source.clip = Resources.Load ("Audio/"+fileName) as AudioClip;
			source.loop = loop;
			source.volume = volume;

			source.Play ();
		
			if (loop == false) 
			{
				GameObject.Destroy (sound, source.clip.length);
			} 
			else 
			{
				m_ptrSounds.Add (sound);
			}
			return source;
		} 
		else
		{
			return null;
		}
	}

	public void StopAllSounds()
	{
		for(int i = m_ptrSounds.Count-1; i > -1; i--)
		{
            if (m_ptrSounds[i] != null)
            {
                m_ptrSounds[i].GetComponent<AudioSource>().Stop();
                Object.Destroy(m_ptrSounds[i], Time.fixedDeltaTime);
            }
		}
	}
	
	public void StopSound(string fileName)
	{
		int removeSoundIndex = -1;
		
		for(int i = m_ptrSounds.Count-1; i > -1; i--)
		{
			if(m_ptrSounds[i].name.Equals(new StringBuilder("Audio: ").Append(fileName).ToString()))
			{
				m_ptrSounds[i].GetComponent<AudioSource>().Stop();
				removeSoundIndex = i;
				break;
			}
		}
		
		if(removeSoundIndex != -1)
		{
			GameObject sound = m_ptrSounds[removeSoundIndex];
			m_ptrSounds.RemoveAt(removeSoundIndex);
			Object.Destroy(sound);
		}
		
	}


	public void PauseSound(string fileName)
	{
		int removeSoundIndex = -1;
		
		for(int i = m_ptrSounds.Count-1; i > -1; i--)
		{
			if(m_ptrSounds[i].name.Equals(new StringBuilder("Audio: ").Append(fileName).ToString()))
			{
				m_ptrSounds[i].GetComponent<AudioSource>().Pause();
				break;
			}
		}
		
		if(removeSoundIndex != -1)
		{
			GameObject sound = m_ptrSounds[removeSoundIndex];
			m_ptrSounds.RemoveAt(removeSoundIndex);
			Object.Destroy(sound);
		}
	}

	public void ResumeSound(string fileName)
	{
		int removeSoundIndex = -1;
		
		for(int i = m_ptrSounds.Count-1; i > -1; i--)
		{
			if(m_ptrSounds[i].name.Equals(new StringBuilder("Audio: ").Append(fileName).ToString()))
			{
				m_ptrSounds[i].GetComponent<AudioSource>().Play();
				break;
			}
		}
		
		if(removeSoundIndex != -1)
		{
			GameObject sound = m_ptrSounds[removeSoundIndex];
			m_ptrSounds.RemoveAt(removeSoundIndex);
			Object.Destroy(sound);
		}
	}

	public void StartAllSounds()
	{
		for(int i = m_ptrSounds.Count-1; i > -1; i--)
		{
			m_ptrSounds[i].GetComponent<AudioSource>().Play();
		}
	}

	public void Destroy()
	{
//		AudioSource[] audios = GameObject.FindObjectsOfType<AudioSource> () as AudioSource[];
//
//		for (int i=0; i<audios.Length; i++)
//						GameObject.Destroy (audios [i].gameObject);
//
//		for(int i=0; i< m_ptrSounds.Count; i++)
//		{
//			GameObject.Destroy(m_ptrSounds[i]);
////			m_ptrSounds.RemoveAt(0);
//		}
//		m_ptrSounds = null;
	}
}


