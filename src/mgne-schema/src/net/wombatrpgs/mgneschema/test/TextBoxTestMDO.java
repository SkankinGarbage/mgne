/**
 *  TextBoxTest.java
 *  Created on Feb 2, 2013 4:33:39 AM for project RainfallSchema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgneschema.test;

import net.wombatrpgs.mgneschema.test.data.TestState;
import net.wombatrpgs.mgneschema.ui.FontMDO;
import net.wombatrpgs.mgneschema.ui.TextBoxMDO;
import net.wombatrpgs.mgns.core.Annotations.Desc;
import net.wombatrpgs.mgns.core.Annotations.Path;
import net.wombatrpgs.mgns.core.Annotations.SchemaLink;
import net.wombatrpgs.mgns.core.MainSchema;

/**
 * Testing scheme for text boxes.
 */
@Path("test/")
public class TextBoxTestMDO extends MainSchema {
	
	public TestState enabled;
	
	@Desc("Text box to display")
	@SchemaLink(TextBoxMDO.class)
	public String box;
	
	@Desc("Font to use")
	@SchemaLink(FontMDO.class)
	public String font;
	
	@Desc("Text to display")
	public String text;

}
