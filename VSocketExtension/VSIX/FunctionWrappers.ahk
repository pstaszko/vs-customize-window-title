IniRead(Filename, Section, Key, Default){
	IniRead OutputVar, %Filename%, %Section%, %Key%, %Default%
	return OutputVar
}
;;;
IniWrite(Filename, Section, Key, Value){
	IniWrite %Value%, %Filename%, %Section%, %Key%
}
