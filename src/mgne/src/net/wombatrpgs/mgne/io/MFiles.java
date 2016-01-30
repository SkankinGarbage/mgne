/**
 *  FileLoader.java
 *  Created on Apr 18, 2013 7:18:53 PM for project rainfall-libgdx
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.io;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.PrintWriter;
import java.util.List;

import net.wombatrpgs.mgne.core.Constants;
import net.wombatrpgs.mgne.core.MGlobal;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.files.FileHandle;

/**
 * Just a thing that sits in MGlobal for easy text file access. Don't load
 * anything expensive with these; they're really only intended to be used for
 * simple config files and shaders.
 */
public class MFiles {
	
	/**
	 * Loads up a file then reads it as a string. Processes the file as being
	 * internal.
	 * @param 	fileName		The name of the file to load, relative to the
	 * 							game directory, ie "res/shaders/a.txt"
	 * @return					That file's contents as a string, or empty if
	 * 							the file is not found
	 */
	public String getText(String fileName) {
		FileHandle handle = getExistingHandle(fileName);
		if (handle != null) {
			return handle.readString();
		} else {
			return "";
		}
	}
	
	/**
	 * Loads up a file then returns it as an input stream. Relative to internal.
	 * @param	fileName		The name of the file to load relative to the
	 * 							game directory, ie "saves/bob/player.sav"
	 * @return					An input stream from that file, or null if file
	 * 							is not found
	 */
	public InputStream getInputStream(String fileName) {
		FileHandle handle = getExistingHandle(fileName);
		if (handle != null) {
			return handle.read();
		} else {
			return null;
		}
		
	}
	
	/**
	 * Creates a file for writing. Overwrites if it already exists. Relative to
	 * the local namespace.
	 * @param	fileName		The name of the file to write relative to the
	 * 							game directory, ie "saves/bob/player.sav"
	 * @return					An output stream to write to that file
	 */
	public OutputStream getOutputStream(String fileName) {
		FileHandle handle = Gdx.files.local(fileName);
		return handle.write(false);
	}
	
	/**
	 * Writes the current values in the args global into the config file.
	 * @param configs			The names of the configs to write
	 */
	public void writeConfigs(List<String> configs) {
		OutputStream stream = getOutputStream(Constants.CONFIG_FILE);
		PrintWriter out = new PrintWriter(stream);
		
		try {
			// hacky as fuck, this is really tacked on, sorry
			for (String config : configs) {
				String value = MGlobal.args.get(config);
				if (value != null) {
					out.write(config + " = " + value + "\n");
				}
			}
			
			out.flush();
			out.close();
			stream.close();
		} catch (IOException e) {
			MGlobal.reporter.err("Error writing configs", e);
		}
	}
	
	/**
	 * Internal loader for files. Gets the handle, checks if the handle exists,
	 * and returns it if it does. Errors if file not found.
	 * @param	fileName		The path to the file relative to internals
	 * @return					A handle for that file
	 */
	protected FileHandle getExistingHandle(String fileName) {
		FileHandle handle = Gdx.files.internal(fileName);
		if (!handle.exists()) {
			MGlobal.reporter.err("File not found: " + fileName);
			return null;
		} else {
			return handle;
		}
	}
	
	/**
	 * Checks if a file handle exists.
	 * @param	fileName		The name of the file to check
	 * @return					True if the file exists, false if not
	 */
	public boolean checkIfFileExists(String fileName) {
		FileHandle handle = Gdx.files.internal(fileName);
		return handle.exists();
	}

}
