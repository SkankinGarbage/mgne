/**
 *  OutputHandler.java
 *  Created on Aug 8, 2012 4:15:13 AM for project MGNSE
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgnse.io;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.zip.ZipEntry;
import java.util.zip.ZipOutputStream;

import net.wombatrpgs.mgns.core.CompactData;
import net.wombatrpgs.mgns.core.CompactDataList;
import net.wombatrpgs.mgns.core.MainSchema;
import net.wombatrpgs.mgns.core.Schema;
import net.wombatrpgs.mgnse.Global;
import net.wombatrpgs.mgnse.MainFrame;
import net.wombatrpgs.mgnse.schema.AppConfig;
import net.wombatrpgs.mgnse.schema.ProjectConfig;

import com.fasterxml.jackson.core.JsonProcessingException;

/**
 * Handles writing everything. This usually means dealing with Jackson.
 */
public class OutputHandler {
	
	private static final String APP_CONFIG_FILE_NAME = "jgdbe.cfg";
	
	protected MainFrame parent;
	protected File appConfigFile, projectConfigFile;
	
	/**
	 * Creates a new output handler that needs to have its files set! It does
	 * load up a default app config though.
	 * @param parent The parent application frame
	 */
	public OutputHandler(MainFrame parent) {
		this.appConfigFile = new File(APP_CONFIG_FILE_NAME);
		this.parent = parent;
	}
	
	/** @param f The new app config file */
	public void setAppConfigFile(File f) { this.appConfigFile = f; }
	/** @return The current app config file */
	public File getAppConfigFile() { return appConfigFile; }
	
	/** @param f The new project config file */
	public void setProjectConfigFile(File f) { this.projectConfigFile = f; }
	/** @return The current project config file */
	public File getProjectConfigFile() { return projectConfigFile; }
	
	/**
	 * Writes the global application config file.
	 * @param config The data structure underlying to file.
	 */
	public void writeAppConfig(AppConfig config) {
		try {
			Global.instance().writer().writeValue(appConfigFile, config);
		} catch (JsonProcessingException e) {
			Global.instance().err("Malformatted internal project config", e);
		} catch (IOException e) {
			Global.instance().err("Couldn't write mgnse.cfg", e);
		}
	}

	/**
	 * In charge of writing the schema to the location specified.
	 * @param s The schema to write
	 * @param file The file to write it to
	 */
	public void writeSchema(Schema s, File file) {
		try {
			Global.instance().writer().writeValue(file, s);
			Global.instance().debug("Wrote to " + file);
		} catch (JsonProcessingException e) {
			Global.instance().err("Malformatted schema file at " + file, e);
		} catch (IOException e) {
			Global.instance().err("Couldn't write to " + file, e);
		}
	}
	
	/**
	 * Deletes a file from the file system.
	 * @param f
	 */
	public void delete(File f) {
		f.delete();
	}

	/**
	 * Writes a brand-new schema. For use in wizards.
	 * @param mdo The mdo of the schema to write
	 */
	public void writeNewSchema(MainSchema mdo) {
		InputHandler in = parent.getLogic().getIn();
		OutputHandler out = parent.getLogic().getOut();
		ProjectConfig projectConfig = parent.getLogic().getConfig();
		String path = in.getFile(
				out.getProjectConfigFile().getParentFile(), 
				projectConfig.data).getAbsolutePath() + "\\";
		path += mdo.getClass().getName().replace('.', '\\') + "\\";
		path += mdo.key + ".json";
		File file = new File(path);
		try {
			file.getParentFile().mkdirs();
			file.createNewFile();
			writeSchema(mdo, file);
		} catch (IOException e) {
			Global.instance().err("Error creating " + file, e);
		}
	}
	
	/**
	 * Exports compact project data.
	 */
	public void export() {
		InputHandler in = parent.getLogic().getIn();
		ProjectConfig projectConfig = parent.getLogic().getConfig();
		
		List<File> allFiles = in.recursivelyGetFiles(parent.getLogic().loadFile(projectConfig.data));
		ArrayList<File> toExport = new ArrayList<File>();
		for (File file : allFiles) {
			if (file.getName().endsWith(".json")) {
				toExport.add(file);
			}
		}
		
		String path = in.getFile(
				getProjectConfigFile().getParentFile(), 
				projectConfig.data).getAbsolutePath() + File.separator +
				projectConfig.name + ".jgdb";
		
		try {
			CompactDataList compact = new CompactDataList();
			compact.data = new CompactData[toExport.size()];
			for (int i = 0; i < toExport.size(); i += 1) {
				File file = toExport.get(i);
				CompactData entry = new CompactData();
				Class<? extends MainSchema> schemaClass = parent.getLogic().getSchemaTree().getSchemaByFile(file);
				MainSchema data = in.instantiateData(schemaClass, file);
				data.description = "";
				data.name = "";
				entry.schemaName = schemaClass.getCanonicalName();
				entry.dataString = Global.instance().writer().writeValueAsString(data).replaceAll("\\s+","");
				compact.data[i] = entry;
			}
			
			File blob = new File(path);
			blob.getParentFile().mkdirs();
			blob.createNewFile();
			FileOutputStream fileOut = new FileOutputStream(blob);
			ZipOutputStream zipOut = new ZipOutputStream(fileOut);
			ZipEntry entry = new ZipEntry(CompactData.DATA_FILE_NAME);
			zipOut.putNextEntry(entry);
			Global.instance().writer().writeValue(zipOut, compact);
			zipOut.flush();
			zipOut.close();
			
		} catch (IOException e) {
			Global.instance().err(e.getMessage(), e);
		}
	}

}
