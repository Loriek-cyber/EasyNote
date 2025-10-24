//this has a buiness logic that two documents can't have the same id and path -> same as windows but better
export class Document {
    constructor(id, title, content,path) {
        this.id = id;
        this.title = title;
        this.content = content;
        this.path = path;
    }

    toJSON() {
        return {
            id: this.id,
            title: this.title,
            content: this.content,
            path: this.path
        };}
    
    equals(other) {
        return other instanceof Document &&
               this.id === other.id &&
               this.title === other.title &&
               this.content === other.content &&
               this.path === other.path;}
    }