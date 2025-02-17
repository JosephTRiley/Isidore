% Data pushed by NetTest.cs

matDir = 'Mats';
figDir = 'Figs';
if(~exist(matDir,'dir')); mkdir(matDir); end
if(~exist(figDir,'dir')); mkdir(figDir); end

%% Checks that the source and extracted arrays are equal
drArr = rArr - reArr; % Array difference
passedVec = all(abs(drArr) <= 5 * eps, "all");

%% Now for a 2D array
darArr = arArr - areArr; % Array difference
passedArr = all(abs(darArr) <= 5 * eps, "all");

%% Checks that the single value arrays match
passedBool = all(sArr' == seArr, "all");

%% Repeat the 2D array process completely from Matlab
% DLL location (Be sure to run in x64 debug)
addpath("../../../bin/x64/Debug");
% Initializes DLLs
warning off MATLAB:NET:AddAssembly:nameConflict
nameSpaces = {"Isidore.Maths", "Isidore.Matlab"};
assName = {"maths", "ml"};
for idx = 1:length(nameSpaces)
    fullPath = which(nameSpaces{idx} + ".dll");
    try
        eval(assName{idx} + " = NET.addAssembly(fullPath);");
        eval("import " + nameSpaces{idx} + ".*;");
    catch e
        e.message
        if(isa(e, "NET.NetException"))
            e.ExceptionObject
        end
    end
end

%% Populates an array with random vectors
len0 = 100;
len1 = 100;
r = rand(len0, len1, 3); % Random numbers
vArr = NET.createArray("Isidore.Maths.Vector", [len0, len1]); % Vec array
for idx0 = 1 : len0
    for idx1 = 1 : len1
        v = Isidore.Maths.Vector(squeeze(r(idx0, idx1, :)));
        vArr(idx0, idx1) = v;
    end
end

%% Extracts the vector components from the array
rNet = NET.invokeGenericMethod("Isidore.Matlab.Net", "GetValue", ...
    {'Isidore.Maths.Vector', 'System.Double'}, vArr, "Comp");
rMat = double(rNet);
passedMat = all(rMat == r, "all");

%% Saves
save([matDir,'\NetCheck'])