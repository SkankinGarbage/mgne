/**
 *  TextBoxMDO.java
 *  Created on Feb 2, 2013 3:35:57 AM for project RainfallSchema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.sagaschema.ui;

import net.wombatrpgs.mgns.core.Annotations.Desc;
import net.wombatrpgs.mgns.core.Annotations.Nullable;
import net.wombatrpgs.mgns.core.Annotations.Path;
import net.wombatrpgs.mgns.core.Annotations.SchemaLink;
import net.wombatrpgs.mgns.core.MainSchema;
import net.wombatrpgs.sagaschema.audio.SoundMDO;
import net.wombatrpgs.sagaschema.ui.data.AnchorType;

/**
 * Defines a textbox.
 */
@Path("ui/")
public class TextBoxMDO extends MainSchema {
	
	@Desc("Anchor type, like where this displays on page")
	public AnchorType anchor;
	
	@Desc("Nineslice - will be stretched to form backer for box")
	@SchemaLink(NinesliceMDO.class)
	@Nullable
	public String nineslice;
	
	@Desc("Text autotype speed - in characters per second")
	public Integer typeSpeed;
	
	@Desc("Type sfx - plays once per character autotyped")
	@SchemaLink(SoundMDO.class)
	@Nullable
	public String typeSfx;
	
	@Desc("Line count")
	public Integer lines;
	
	@Desc("Pixel amount on each side from edge of the screen")
	public Integer marginWidth;
	
	@Desc("Pixel amount on top and bottom from edge of the screen")
	public Integer marginHeight;

}
