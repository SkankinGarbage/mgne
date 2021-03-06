/**
 *  SceneParser.java
 *  Created on Mar 31, 2014 12:49:26 PM for project mgne
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.scenes;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import net.wombatrpgs.mgne.core.MAssets;
import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.core.interfaces.FinishListener;
import net.wombatrpgs.mgne.core.interfaces.Queueable;
import net.wombatrpgs.mgne.core.interfaces.Updateable;
import net.wombatrpgs.mgne.io.CommandListener;
import net.wombatrpgs.mgne.io.CommandMap;
import net.wombatrpgs.mgne.io.command.CMapScene;
import net.wombatrpgs.mgne.maps.Level;
import net.wombatrpgs.mgne.screen.Screen;
import net.wombatrpgs.mgneschema.io.data.InputCommand;

/**
 * Represents all classes that parse scenes. This is common between file-loaded
 * scenes and those interpreted from strings or whatever on maps. The only thing
 * it's missing is how the commandMap gets populated.
 */
public class SceneParser implements	Updateable,
									CommandListener,
									Queueable {
	
	protected static int runningCount;
	
	protected Screen parent;
	protected List<FinishListener> listeners;
	protected CommandMap commandMap;
	
	protected List<SceneCommand> commands;
	protected Iterator<SceneCommand> runningCommands;
	protected SceneCommand currentCommand;
	
	protected boolean running;
	
	/**
	 * Creates a scene parser with no commands. It's expected that somewhere
	 * along the way it will be filled in with some commands. This is useful for
	 * generating dialog on the fly.
	 */
	public SceneParser() {
		this.running = false;
		this.listeners = new ArrayList<FinishListener>();
	}
	
	/** @return The screen we're running on */
	public Screen getScreen() { return parent; }

	/**
	 * @see net.wombatrpgs.mgne.maps.MapThing#update(float)
	 */
	@Override
	public void update(float elapsed) {
		if (running) {
			if (currentCommand.isFinished()) {
				nextCommand();
			} else {
				currentCommand.update(elapsed);
			}
		}
	}

	/**
	 * @see net.wombatrpgs.mgne.io.CommandListener#onCommand
	 * (net.wombatrpgs.mgneschema.io.data.InputCommand)
	 */
	@Override
	public boolean onCommand(InputCommand command) {
		if (!running || currentCommand == null) {
			MGlobal.reporter.warn("Received a command to invalid parser " + this);
			return false;
		}
		return currentCommand.onCommand(command);
	}
	
	/**
	 * @see net.wombatrpgs.mgne.core.interfaces.Queueable#queueRequiredAssets
	 * (MAssets)
	 */
	@Override
	public void queueRequiredAssets(MAssets manager) {
		// subclasses might want to do something here
	}

	/**
	 * @see net.wombatrpgs.mgne.core.interfaces.Queueable#postProcessing
	 * (MAssets, int)
	 */
	@Override
	public void postProcessing(MAssets manager, int pass) {
		if (pass == 0) {
			manager.requestLoading();
			for (SceneCommand command : commands) {
				command.addToQueue();
				command.queueRequiredAssets(manager);
			}
		} else {
			for (SceneCommand command : commands) {
				command.postProcessing(manager, pass - 1);
			}
			List<SceneCommand> replacementSet = new ArrayList<SceneCommand>();
			List<SceneCommand> currentSet = new ArrayList<SceneCommand>();
			for (SceneCommand command : commands) {
				if (!currentSet.isEmpty() && !command.getID().equals(currentSet.get(0).getID())) {
					replacementSet.addAll(currentSet.get(0).merge(currentSet));
					currentSet.clear();
				}
				currentSet.add(command);
			}
			if (!currentSet.isEmpty()) {
				replacementSet.addAll(currentSet.get(0).merge(currentSet));
			}
			commands = replacementSet;
		}
	}

	/** @return Trus if this parser is in the process of running */
	public boolean isRunning() { return this.running; }
	
	/** @return The level currently active. Same as the static call */
	public Level getLevel() { return MGlobal.levelManager.getActive(); }
	
	/** @return True if any scene parsers are running */
	public static boolean anyRunning() { return runningCount > 0; }

	/**
	 * Runs the scene assuming it should be run in the current context. The only
	 * check is to make sure it isn't already running. This always runs on the
	 * current screen.
	 */
	public void run() {
		if (running) {
			MGlobal.reporter.warn("Trying to run a running scene: " + this);
			return;
		}
		if (commands.size() == 0) {
			MGlobal.reporter.warn("Tried to run an empty scene: " + this);
		}
		
		MGlobal.reporter.inform("Now running a scene: " + this);
		commandMap = new CMapScene();
		parent = MGlobal.screens.peek();
		parent.addUChild(this);
		parent.pushCommandContext(commandMap);
		parent.pushCommandListener(this);
		runningCommands = commands.listIterator();
		nextCommand();
		running = true;
		runningCount += 1;
	}
	
	/**
	 * Adds a listener to the parser. The parser will be notified when the
	 * parser finished executing the scene. The listener will then be removed,
	 * so watch out.
	 * @param 	listener		The listener to add
	 */
	public void addListener(FinishListener listener) {
		listeners.add(listener);
	}
	
	/**
	 * Removes a listener to a parser. It will no longer be notified.
	 * @param 	listener		The existing listener to remove
	 */
	public void removeListener(FinishListener listener) {
		if (listeners.contains(listener)) {
			listeners.remove(listener);
		} else {
			MGlobal.reporter.warn("Tried to remove a non-listener: " + listener);
		}
	}
	
	/**
	 * Adds a bunch of scene commands to this parser.
	 * @param	newCommands		The list of commands to add		
	 */
	public void addSceneCommands(List<SceneCommand> newCommands) {
		if (commands == null) {
			commands = new ArrayList<SceneCommand>();
		}
		commands.addAll(newCommands);
	}
	
	/**
	 * Cuts short this parser's execution. Meant to be called by its children.
	 */
	public void abort() {
		terminate(true);
	}
	
	/**
	 * Called when this parser finishes execution.
	 * @param	forced			True if the termination was unexpected
	 */
	protected void terminate(boolean forced) {
		MGlobal.reporter.inform("Terminated a scene: " + this);
		parent.removeCommandContext(commandMap);
		parent.removeCommandListener(this);
		parent.removeUChild(this);
		running = false;
		runningCount -= 1;
		if (!forced) {
			for (FinishListener listener : listeners) {
				listener.onFinish();
			}
		}
		listeners.clear();
	}
	
	/**
	 * Moves on to start processing the next command.
	 */
	protected void nextCommand() {
		if (runningCommands.hasNext()) {
			currentCommand = runningCommands.next();
			currentCommand.run(this);
		} else {
			terminate(false);
		}
	}
}
