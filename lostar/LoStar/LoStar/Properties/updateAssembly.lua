require("iuplua")
ml = iup.multiline{expand="YES", border="YES"}
dlg = iup.dialog{ml; title="Add a comment about the version", size="QUARTERxQUARTER"}
dlg:show()

if (iup.MainLoopLevel()==0) then
  iup.MainLoop()
end

local comment  = ml.value
if comment:len()==0 then
	comment = nil
end

function exec(cmd)
        io.stderr:write("Execute cmd ["..cmd.."]\n")
        os.execute(cmd)
end


--function dolog(s)
--	local ff = io.open([[C:\Users\FR021201\blob.txt]],"a")
--	ff:write(s)
--	ff:close()
--end

function lines(str)
  local t = {}
  local function helper(line) table.insert(t, line) return "" end
  helper((str:gsub("(.-)\r?\n", helper)))
  return t
end

function getVersion(path)
        io.stderr:write("Path "..path.."\n")
	local bInsert = false;
	local newVersion = ""
	local fou = io.open(path,"w")
	local fin = io.open(path..".bak")
	while true do
		local l = fin:read()
		if not l then
			break
		end
		local m = l:match("AssemblyVersion(%(.%d*%.%d*%.%d*%.%d*.%))")
		if m then
			local ma, mi, build = m:match("(%d*)%.(%d*)%.%d*%.(%d*)")
			build = tonumber(build)+1
			newVersion = [["]]..ma..[[.]]..mi..[[.0.]]..build..[["]]
			l = "[assembly: AssemblyVersion("..newVersion..")]"
		end
		local m = l:match("AssemblyFileVersion(%(.%d*%.%d*%.%d*%.%d*.%))")
		if m then
			l = "[assembly: AssemblyFileVersion("..newVersion..")]"
			bInsert= true
		end
		fou:write(l)
		fou:write("\n")
		if bInsert then
			fou:write("\n////  "..newVersion.."  Compiled "..os.date("%d.%m.%Y %H:%M"))
			if comment then
				fou:write("\n")
				comments = lines(comment)
				for _,v in ipairs(comments) do
					fou:write("//// "..v.."\n")
				end
			end
			bInsert = false;
		end
	end

	fin:close()
	fou:close()
end

local assy =arg[1]..[[\Properties\AssemblyInfo.cs]]
exec([[copy "]]..assy..[[" "]]..assy..[[.bak"]]);
getVersion(assy)
os.exit(0)
