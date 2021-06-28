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
imagesc(pos0, pos1, reflect); 
axis xy; axis image; colorbar; 
title('Reflection')

cnt = cnt + 1;
subplot(3,3,cnt) 
imagesc(pos0, pos1, temp); 
axis xy; axis image; colorbar; 
title('Temperature')

cnt = cnt + 1;
subplot(3,3,cnt) 
imagesc(pos0, pos1, temp1); 
axis xy; axis image; colorbar; 
title('Temperature 1')

cnt = cnt + 1;
subplot(3,3,cnt) 
imagesc(pos0, pos1, temp2); 
axis xy; axis image; colorbar; 
title('Temperature 2')

cnt = cnt + 1;
subplot(3,3,cnt)
imagesc(pos0, pos1,depth); 
axis xy; axis image; colorbar; 
title('Depth')

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

%% Saves
save('MatTraceReflectSphere')