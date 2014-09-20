/**
 *  SoundManager.java
 *  Created on Sep 14, 2014 9:58:31 PM for project mgne
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.io.audio;

import java.util.HashMap;
import java.util.Map;

import net.wombatrpgs.mgne.core.AssetQueuer;
import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.graphics.interfaces.Disposable;
import net.wombatrpgs.mgneschema.audio.SoundManagerMDO;
import net.wombatrpgs.mgneschema.audio.data.SoundManagerEntryMDO;

/**
 * General purpose sfx queuer and player. Lives in global land and plays a file
 * based on short ID string.
 */
public class SoundManager extends AssetQueuer implements Disposable {
	
	protected static final String KEY_DEFAULT = "soundmanager_default";
	
	protected SoundManagerMDO mdo;
	protected Map<String, SoundObject> sounds;
	
	/**
	 * Creates a sound manager from data.
	 * @param	mdo				The data to create from
	 */
	public SoundManager(SoundManagerMDO mdo) {
		this.mdo = mdo;
		
		sounds = new HashMap<String, SoundObject>();
		for (SoundManagerEntryMDO entryMDO : mdo.entries) {
			SoundObject sound = SoundObject.createFromFile(entryMDO.file);
			sounds.put(entryMDO.key, sound);
			assets.add(sound);
		}
	}
	
	/**
	 * Creates a sound manager from the default MDO.
	 */
	public SoundManager() {
		this(MGlobal.data.getEntryFor(KEY_DEFAULT, SoundManagerMDO.class));
	}

	/**
	 * @see net.wombatrpgs.mgne.graphics.interfaces.Disposable#dispose()
	 */
	@Override
	public void dispose() {
		for (SoundObject sound : sounds.values()) {
			sound.dispose();
		}
	}
	
	/**
	 * Plays the sound effect with the given key.
	 * @param	key				The arbitrary tag string given in data for sfx
	 */
	public void play(String key) {
		SoundObject sound = sounds.get(key);
		if (sound == null) {
			MGlobal.reporter.warn("Sound not found for key: " + key);
		} else {
			sound.play();
		}
	}

}