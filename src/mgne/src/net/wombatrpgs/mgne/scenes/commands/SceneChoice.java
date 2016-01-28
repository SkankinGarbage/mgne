/**
 *  SceneChoice.java
 *  Created on Jun 7, 2015 10:55:28 PM for project mgne
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.scenes.commands;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.core.interfaces.FinishListener;
import net.wombatrpgs.mgne.io.CommandListener;
import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;
import net.wombatrpgs.mgne.screen.Screen;
import net.wombatrpgs.mgne.screen.ScreenObject;
import net.wombatrpgs.mgne.ui.Nineslice;
import net.wombatrpgs.mgne.ui.text.TextFormat;
import net.wombatrpgs.mgneschema.io.data.InputCommand;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.lib.ZeroArgFunction;

import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.utils.Align;

/**
 * A total hack that asks a yes/no question. Choosing YES continues, choosing
 * NO cancels.
 * Usage: {@code choice()}
 */
public class SceneChoice extends ZeroArgFunction {
	
	/**
	 * @see org.luaj.vm2.lib.ZeroArgFunction#call()
	 */
	@Override
	public LuaValue call() {
		SceneLib.addFunction(new SceneCommand() {
			
			boolean shouldFinish;
			
			@Override protected void internalRun() {
				shouldFinish = false;
				Choice choice = new Choice(new FinishListener() {
					@Override public void onFinish() {
						shouldFinish = true;
					}
				},
				new FinishListener() {
					@Override public void onFinish() {
						parent.abort();
						shouldFinish = true;
					}
				});
				MGlobal.assets.loadAsset(choice, "choice");
				MGlobal.screens.peek().addChild(choice);
			}
			
			@Override protected boolean shouldFinish() {
				return shouldFinish;
			}
			
		});
		return LuaValue.NIL;
	}
	
	private class Choice extends ScreenObject {
		
		private static final int POS_X = 0;
		private static final int POS_Y = 0;
		private static final int WIDTH = 144;
		private static final int HEIGHT = 48;
		
		private static final String CHOICE_YES = "YES";
		private static final String CHOICE_NO = "NO";
		
		private FinishListener confirm, cancel;
		
		private Nineslice bg;
		private TextFormat format;
		private CommandListener listener;
		private boolean selected;
		
		public Choice(FinishListener onConfirm, FinishListener onCancel) {
			confirm = onConfirm;
			cancel = onCancel;
			
			bg = new Nineslice(WIDTH, HEIGHT);
			assets.add(bg);
			
			format = new TextFormat();
			format.x = POS_X + 38;
			format.y = POS_Y + 20;
			format.width = WIDTH;
			format.height = HEIGHT;
			format.align = Align.left;
			selected = true;
			
			listener = new CommandListener() {
				@Override public boolean onCommand(InputCommand command) {
					switch (command) {
					case MOVE_UP: case MOVE_DOWN:
						selected = !selected;
						return true;
					case UI_CANCEL:
						cancel();
						return true;
					case UI_CONFIRM:
						tryConfirm();
						return true;
					default: return false;
					}
				}
			};
		}

		@Override public int getWidth() { return WIDTH; }

		@Override public int getHeight() { return HEIGHT; }

		@Override public void render(SpriteBatch batch) {
			super.render(batch);
			bg.renderAt(batch, POS_X, POS_Y);
			MGlobal.ui.getFont().draw(batch, format, CHOICE_YES, 16);
			MGlobal.ui.getFont().draw(batch, format, CHOICE_NO, 0);
			if (selected) {
				MGlobal.ui.getCursor().renderAt(batch, 20, 21);
			} else {
				MGlobal.ui.getCursor().renderAt(batch, 20, 5);
			}
			
		}

		@Override public void onAddedToScreen(Screen screen) {
			super.onAddedToScreen(screen);
			screen.pushCommandListener(listener);
		}

		@Override public void onRemovedFromScreen(Screen screen) {
			super.onRemovedFromScreen(screen);
			screen.removeCommandListener(listener);
		}
		
		private void tryConfirm() {
			if (selected) {
				confirm();
			} else {
				cancel();
			}
		}
		
		private void confirm() {
			confirm.onFinish();
			MGlobal.screens.peek().removeChild(this);
		}
		
		private void cancel() {
			cancel.onFinish();
			MGlobal.screens.peek().removeChild(this);
		}
		
	}

}
