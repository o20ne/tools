<?php
/***
* @todo: search my 7 years old files and add those tools in
***/


class PersonalTools
{
	public static $ImageExt = array("gif", "jpeg", "png", "jpg", "bmp");
	public static $ImageMime = array("image/gif", "image/jpeg", "image/png", "image/x-png", "image/jpg", "image/pjpeg");
	

	/**
	 * Returns the cleaned filename
	 * @param string $filename Original filename
	 * @param array $options allow {prefix, suffix, maxlength} as optional parameters
	 * @return string cleaned filename
	 */
	public static function CleanFilename($filename, $options = array())
	{
		$filename = strtolower($filename);
		$filename = str_replace(' ', '-', $filename); // Replaces all spaces with hyphens.
		$filename= preg_replace('/[^A-Za-z0-9\-.]/', '', $filename); // Removes special chars.
		$file_parts = pathinfo($filename);

		if(array_key_exists('prefix', $options))
		{
			$filename = $options['prefix'] . $filename;
		}
		$file_parts = pathinfo($filename);
		if(array_key_exists('suffix', $options))
		{
			$filename = substr($filename, 0, strlen($filename) - strlen($file_parts['extension']) - 1)  . $options['suffix'] . '.' . $file_parts['extension'];
		}
		$file_parts = pathinfo($filename);
		if(array_key_exists('maxlength', $options) && strlen($filename) > $options['maxlength'])
		{
			$filename = substr($filename, 0, $options['maxlength'] - strlen($file_parts['extension']) - 1) . '.' . $file_parts['extension'];
		}

	    return $filename;
	}

	public static function RandomString($length) {
	  $random = '';
	  for ($i = 0; $i < $length; $i++) {
		$random .= rand(0, 1) ? rand(0, 9) : chr(rand(ord('a'), ord('z')));
	  }
	  return $random;
	}
}