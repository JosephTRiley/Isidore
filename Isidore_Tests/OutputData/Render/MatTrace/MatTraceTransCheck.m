% Data pushed by SphereSurfaceTrace.cs
% inter = Intesection map
% reflect = Reflection map
% depth = Depth map
% u = U coordinates for all traces
% v = V coordinates for all traces
% x = Ray's origin in world space X-axis
% y = Ray's origin in world space Y-axis
% pos0 = Grid pixel axis 0 locations [m]
% pos1 = Grid pixel axis 1 locations [m]

% close all; % So I can run it from MatLab

%% Saves
save('MatTraceTransSphere')

%% Displays data
figure; 

cnt = 1;
subplot(3,3,cnt)
imagesc(pos0, pos1, inter); 
axis xy; axis image; colorbar; 
title('Intersection')

cnt = cnt + 1;
subplot(3,3,cnt) 
imagesc(pos0, pos1, cosIncImg); 
axis xy; axis image; colorbar; 
title('cos(Incidence Angle)')

cnt = cnt + 1;
subplot(3,3,cnt) 
imagesc(pos0, pos1, rayRank); 
axis xy; axis image; colorbar; 
title('Ray Rank')

cnt = cnt + 1;
subplot(3,3,cnt)
imagesc(pos0, pos1,depth); 
axis xy; axis image; colorbar; 
title('Last Ray Travel')

cnt = cnt + 1;
subplot(3,3,cnt)
imagesc(pos0, pos1,u); 
axis xy; axis image; colorbar; 
title('U')

cnt = cnt + 1;
subplot(3,3,cnt)
imagesc(pos0, pos1,v); 
axis xy; axis image; colorbar; 
title('V')

cnt = cnt + 1;
subplot(3,3,cnt)
imagesc(pos0, pos1, id); 
axis xy; axis image; colorbar; 
title('Shape ID')

% Saves figure
savefig('MatTraceTransCheck')

%% Extracted display data
figure
cnt = 0;
res = 3;

cnt = cnt + 1;
subplot(res,res,cnt)
imagesc(pos0, pos1, Hit); 
axis xy; axis image; colorbar; 
title('Intersection')

cnt = cnt + 1;
subplot(res,res,cnt)
imagesc(pos0, pos1, Depth); 
axis xy; axis image; colorbar; 
title('Last Ray Travel')

cnt = cnt + 1;
subplot(res,res,cnt)
imagesc(pos0, pos1, CosInc); 
axis xy; axis image; colorbar; 
title('Cosine Incidence Angle')

cnt = cnt + 1;
subplot(res,res,cnt)
imagesc(pos0, pos1, ID); 
axis xy; axis image; colorbar; 
title('ID')

cnt = cnt + 1;
subplot(res,res,cnt)
imagesc(pos0, pos1, U); 
axis xy; axis image; colorbar; 
title('U')

cnt = cnt + 1;
subplot(res,res,cnt)
imagesc(pos0, pos1, V); 
axis xy; axis image; colorbar; 
title('V')

cnt = cnt + 1;
subplot(res,res,cnt)
imagesc(pos0, pos1, Reflect); 
axis xy; axis image; colorbar; 
title('Reflect')

% Saves figure
savefig('MatTraceTransCheck_ExtractedArray')