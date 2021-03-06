/**
 *  MgnEmuPlayer.java
 *  Created on Oct 9, 2014 6:42:52 PM for project gme-java
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.io.audio;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;

import com.badlogic.gdx.audio.AudioDevice;

import gme.MusicEmu;
import net.wombatrpgs.gme.GmeListener;
import net.wombatrpgs.mgne.core.AssetQueuer;
import net.wombatrpgs.mgne.core.Constants;
import net.wombatrpgs.mgne.core.MAssets;
import net.wombatrpgs.mgne.core.MGlobal;

/**
 * The MGN version of EmuPlayer. Not a manager in itself. Handles all track from
 * a specific gbs file. Even though this class isn't a singleton, it doesn't
 * necessarily play nicely with other instances because the underlying instance
 * uses some static members. The emulator could probably use a rewrite.
 */
public class MgnEmuPlayer extends AssetQueuer implements GmeListener {
	
	protected static final int BUFFER_LENGTH = 8192; // in samples
	
	protected String fileName;
	protected MusicEmu emu;
	protected SoundManager manager;
	protected int track;
	protected boolean playing;
	
	/**
	 * Creates a new player for a specific music package file.
	 * @param	gbsFileName		The path of the .gbs file to load
	 * @param	manager			The sound manager the emu is managed by
	 */
	public MgnEmuPlayer(String gbsFileName, SoundManager manager) {
		this.fileName = Constants.AUDIO_DIR + gbsFileName;
		this.manager = manager;
		
		playing = false;
		track = -1;
	}
	
	/** @return True if a track is playing on this emulator */
	public boolean isPlaying() { return playing; }

	/**
	 * @see net.wombatrpgs.mgne.core.interfaces.Queueable#queueRequiredAssets
	 * (net.wombatrpgs.mgne.core.MAssets)
	 */
	@Override
	public void queueRequiredAssets(MAssets manager) {
		manager.load(fileName, MusicEmu.class);
	}

	/**
	 * @see net.wombatrpgs.mgne.core.interfaces.Queueable#postProcessing
	 * (net.wombatrpgs.mgne.core.MAssets, int)
	 */
	@Override
	public void postProcessing(MAssets manager, int pass) {
		emu = manager.get(fileName, MusicEmu.class);
		emu.setListener(this);
	}

	/**
	 * @see net.wombatrpgs.gme.GmeListener#onError(java.lang.String)
	 */
	@Override
	public void onError(String error) {
		MGlobal.reporter.warn(fileName + ": " + error);
		if (playing) {
			playTrack(track);
		}
	}

	/**
	 * Blocking IO call to play some samples from this emulator to the sound
	 * manager audio device.
	 * @return					True if any samples were written
	 */
	public boolean writeSamples() {
		AudioDevice device = manager.getDevice();
		if (playing && !emu.trackEnded()) {
			try {
				byte [] buffer = new byte [BUFFER_LENGTH * 2];
				short[] shorts = new short[BUFFER_LENGTH];
				int count = emu.play(buffer, BUFFER_LENGTH);
				ByteBuffer.wrap(buffer).order(ByteOrder.BIG_ENDIAN).asShortBuffer().get(shorts);
				device.writeSamples(shorts, 0, count);
			} catch (Exception e) {
				MGlobal.reporter.warn("GB APU exception", e);
				fadeout(0);
				onError(e.getMessage());
			}
			return true;
		}
		return false;
	}
	
	/**
	 * Plays the given track in this file.
	 * @param	track			The index of the track to play
	 */
	public void playTrack(int track) {
		this.emu.startTrack(track);
		this.track = track;
		this.playing = true;
	}
	
	/**
	 * Fades out the BGM in the given amount of seconds.
	 * @param	seconds			The amount of seconds to take to fade
	 */
	public void fadeout(int seconds) {
		emu.setFade(emu.currentTime(), seconds);
	}

}
