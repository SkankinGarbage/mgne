package gme;

import javax.sound.sampled.AudioFormat;
import javax.sound.sampled.AudioSystem;
import javax.sound.sampled.DataLine;
import javax.sound.sampled.FloatControl;
import javax.sound.sampled.SourceDataLine;

/* Copyright (C) 2007-2008 Shay Green. This module is free software; you
can redistribute it and/or modify it under the terms of the GNU Lesser
General Public License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version. This
module is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
details. You should have received a copy of the GNU Lesser General Public
License along with this module; if not, write to the Free Software Foundation,
Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA */

public class EmuPlayer implements Runnable
{
	private int sampleRate = 0;
	AudioFormat audioFormat;
	DataLine.Info lineInfo;
	MusicEmu emu;
	Thread thread;
	volatile boolean playing_;
	SourceDataLine line;
	double volume_ = 1.0;
	
	// Number of tracks
	public int getTrackCount() { return emu.trackCount(); }
	
	// Starts new track playing, where 0 is the first track.
	// After time seconds, the track starts fading.
	public void startTrack( int track, int time ) throws Exception
	{
		pause();
		if ( line != null )
			line.flush();
		emu.startTrack( track );
		emu.setFade( time, 6 );
		play();
	}
	
	// Currently playing track
	public int getCurrentTrack() { return emu.currentTrack(); }
	
	// Number of seconds played since last startTrack() call
	public int getCurrentTime() { return (emu == null ? 0 : emu.currentTime()); }
	
	// Sets playback volume, where 1.0 is normal, 2.0 is twice as loud.
	// Can be changed while track is playing.
	public void setVolume( double v )
	{
		volume_ = v;
		
		if ( line != null )
		{
			FloatControl mg = (FloatControl) line.getControl( FloatControl.Type.MASTER_GAIN );
			if ( mg != null )
				mg.setValue( (float) (Math.log( v ) / Math.log( 10.0 ) * 20.0) );
		}
	}
	
	// Current playback volume
	public double getVolume() { return volume_; }
	
	// Pauses if track was playing.
	public void pause() throws Exception
	{
		if ( thread != null )
		{
			playing_ = false;
			thread.join();
			thread = null;
		}
	}
	
	// True if track is currently playing
	public boolean isPlaying() { return playing_; }
	
	// Resumes playback where it was paused
	public void play() throws Exception
	{
		if ( line == null )
		{
			line = (SourceDataLine) AudioSystem.getLine( lineInfo );
			line.open( audioFormat );
			setVolume( volume_ );
		}
		thread = new Thread( this );
		playing_ = true;
		thread.start();
	}
	
	// Stops playback and closes audio
	public void stop() throws Exception
	{
		pause();
		
		if ( line != null )
		{
			line.close();
			line = null;
		}
	}
	
	// Called periodically when a track is playing
	protected void idle() { }
	
// private

	// Sets music emulator to get samples from
	public void setEmu( MusicEmu emu, int sampleRate ) throws Exception
	{
		stop();
		this.emu = emu;
		if ( emu != null && line == null && this.sampleRate != sampleRate )
		{
			audioFormat = new AudioFormat( AudioFormat.Encoding.PCM_SIGNED,
					sampleRate, 16, 2, 4, sampleRate, true );
			lineInfo = new DataLine.Info( SourceDataLine.class, audioFormat );
			this.sampleRate = sampleRate;
		}
	}
	
	public void run()
	{
		line.start();
		
		// play track until stop signal
		byte [] buf = new byte [8192];
		while ( playing_ && !emu.trackEnded() )
		{
			int count = emu.play( buf, buf.length / 2 );
			line.write( buf, 0, count * 2 );
			idle();
		}
		
		playing_ = false;
		line.stop();
	}
}
