% Data pushed by SphereShapeTrace.cs
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

subplot(2,3,1)
imagesc(pos0,pos1,inter); 
axis image; axis xy; colorbar; 
title('Intersection')

subplot(2,3,2) 
imagesc(pos0,pos1,acos(cosIncImg)*180/pi.*double(inter)); 
axis image; axis xy; colorbar; 
title('Inc. Angle')

subplot(2,3,3)
imagesc(pos0,pos1,depth); 
axis image; axis xy; colorbar; 
title('Depth')

subplot(2,3,4)
imagesc(pos0,pos1,u); 
axis image; axis xy; colorbar; 
title('U')

subplot(2,3,5)
imagesc(pos0,pos1,v); 
axis image; axis xy; colorbar; 
title('V')

subplot(2,3,6) 
imagesc(pos0,pos1,acos(cosIncImg)*180/pi.*double(inter)); 
axis image; colorbar; 
title('Reflection')

%% Saves
save('ShapeTraceSphere1')