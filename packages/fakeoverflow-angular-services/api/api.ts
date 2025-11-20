export * from './auth.service';
import { AuthService } from './auth.service';
export * from './me.service';
import { MeService } from './me.service';
export * from './post.service';
import { PostService } from './post.service';
export * from './tags.service';
import { TagsService } from './tags.service';
export const APIS = [AuthService, MeService, PostService, TagsService];
