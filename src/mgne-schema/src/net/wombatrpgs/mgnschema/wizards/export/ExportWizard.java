/**
 *  ExportWizard.java
 *  Created on Dec 31, 2019 12:54:28 AM for project jgdbe
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgnschema.wizards.export;

import java.lang.reflect.Field;
import java.util.ArrayList;
import java.util.List;

import net.wombatrpgs.mgns.core.MainSchema;
import net.wombatrpgs.mgns.core.Schema;
import net.wombatrpgs.mgns.core.Annotations.DefaultValue;
import net.wombatrpgs.mgns.core.Annotations.Desc;
import net.wombatrpgs.mgns.core.Annotations.DisplayName;
import net.wombatrpgs.mgns.core.Annotations.ExcludeFromTree;
import net.wombatrpgs.mgns.core.Annotations.Header;
import net.wombatrpgs.mgns.core.Annotations.InlinePolymorphic;
import net.wombatrpgs.mgns.core.Annotations.InlineSchema;
import net.wombatrpgs.mgns.core.Annotations.Path;
import net.wombatrpgs.mgns.core.Annotations.SchemaLink;
import net.wombatrpgs.mgnse.Global;
import net.wombatrpgs.mgnse.MainFrame;
import net.wombatrpgs.mgnse.tree.SchemaNode;
import net.wombatrpgs.mgnse.wizard.Wizard;

/**
 * Exports all schema as json
 */
public class ExportWizard extends Wizard {

	/** @see net.wombatrpgs.mgnse.wizard.Wizard#getName() */
	@Override public String getName() {
		return "JSON schema export";
	}

	/** @see net.wombatrpgs.mgnse.wizard.Wizard#run() */
	@Override public void run() {
		for (SchemaNode node : Global.instance().getImplementers(Schema.class)) {
			convertSchema(node.getSchema());
		}
	}
	
	private static void convertSchema(Class<? extends MainSchema> clazz) {
		System.out.println("Converting " + clazz + "...");
		
		SchemaClass toSerialize = new SchemaClass();
		toSerialize.name = clazz.getSimpleName();
		List<SchemaField> fields = new ArrayList<SchemaField>();
		
		Path path = clazz.getAnnotation(Path.class);
		if (path != null) {
			toSerialize.path = path.value();
		}
		
		ExcludeFromTree exclude = clazz.getAnnotation(ExcludeFromTree.class);
		if (exclude != null) {
			toSerialize.excludeFromTree = exclude.value();
		}
		
		for (Field field : clazz.getFields()) {
			SchemaField serializeField = new SchemaField();
			fields.add(serializeField);
			
			serializeField.type = field.getType().getSimpleName();
			
			DisplayName displayName = field.getAnnotation(DisplayName.class);
			if (displayName != null) {
				serializeField.name = displayName.value();
			}
			
			DefaultValue defaultValue = field.getAnnotation(DefaultValue.class);
			if (defaultValue != null) {
				serializeField.defaultValue = defaultValue.value();
			}
			
			SchemaLink schemaLink = field.getAnnotation(SchemaLink.class);
			if (schemaLink != null) {
				serializeField.schemaLinkedClass = schemaLink.value().getSimpleName();
			}
			InlineSchema inlineSchema = field.getAnnotation(InlineSchema.class);
			if (inlineSchema != null) {
				serializeField.schemaLinkedClass = inlineSchema.value().getSimpleName();
			}
			InlinePolymorphic polySchema = field.getAnnotation(InlinePolymorphic.class);
			if (polySchema != null) {
				serializeField.schemaLinkedClass = polySchema.value().getSimpleName();
			}
			
			Desc desc = field.getAnnotation(Desc.class);
			if (desc != null) {
				serializeField.description = desc.value();
			}
			
			Header header = field.getAnnotation(Header.class);
			if (header != null) {
				serializeField.header = header.value();
			}
		}
		
		toSerialize.fields = new SchemaField[fields.size()];
		for (int i = 0; i < fields.size(); i += 1) {
			toSerialize.fields[i] = fields.get(i);
		}
		
		MainFrame.instance.getLogic().getOut().writeJson(toSerialize);
	}
}
