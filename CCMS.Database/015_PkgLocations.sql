-------------------------------------------------------------------------
-- PKG_LOCATIONS: Perform CRUD operations on LOCATIONS
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_locations IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_locations (
        po_cursor   OUT sys_refcursor
    );
    
    PROCEDURE p_get_location_details (
        pi_location_id  IN locations.location_id%type,
        po_cursor       OUT sys_refcursor
    );
    
    PROCEDURE p_exists_location (
        pi_location_name    IN locations.location_name%type,
        po_location_id      OUT locations.location_id%type,
        po_deleted          OUT locations.deleted%type
    );

    PROCEDURE p_create_new_location (
        pi_location_name   IN locations.location_name%type,
        po_location_id     OUT locations.location_id%type
    );

    PROCEDURE p_update_location (
        pi_location_id      IN locations.location_id%type,
        pi_location_name    IN locations.location_name%type
    );

    PROCEDURE p_delete_location (
        pi_location_id  IN locations.location_id%type
    );

END pkg_locations;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_locations IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_locations (
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                location_id,
                location_name
            from
                locations
            where
                deleted is null;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_all_locations' || chr(10) || sqlerrm);
    END p_get_all_locations;
-------------------------------------------------------------------------
    PROCEDURE p_get_location_details (
        pi_location_id  IN locations.location_id%type,
        po_cursor       OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                location_id,
                location_name
            from
                locations
            where
                location_id = pi_location_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_location_details - pi_location_id: ' || pi_location_id
                || chr(10) || sqlerrm);
    END p_get_location_details;
-------------------------------------------------------------------------
    PROCEDURE p_exists_location (
        pi_location_name    IN locations.location_name%type,
        po_location_id      OUT locations.location_id%type,
        po_deleted          OUT locations.deleted%type
    ) IS
    BEGIN
        select
            location_id,
            deleted
        into
            po_location_id,
            po_deleted
        from
            locations
        where
            location_name = upper(pi_location_name);
    EXCEPTION
        when no_data_found then
            po_location_id := null;
            po_deleted := null;
        when others then
            raise_application_error(
                -20001,
                'p_exists_location - pi_location_name: ' || pi_location_name
                || chr(10) || sqlerrm);
    END p_exists_location;
-------------------------------------------------------------------------
    PROCEDURE p_create_new_location (
        pi_location_name   IN locations.location_name%type,
        po_location_id     OUT locations.location_id%type
    ) IS
        v_deleted   locations.deleted%type;
    BEGIN
        p_exists_location(pi_location_name, po_location_id, v_deleted);
        if v_deleted is not null then
            update locations
            set deleted = null
            where location_id = po_location_id;
            
            return;
        end if;
        insert into locations (
            location_name
        ) values (
            upper(pi_location_name)
        )
        returning
            location_id
        into
            po_location_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_create_new_location - pi_location_name: ' || pi_location_name
                || chr(10) || sqlerrm);
    END p_create_new_location;
-------------------------------------------------------------------------
    PROCEDURE p_update_location (
        pi_location_id     IN locations.location_id%type,
        pi_location_name   IN locations.location_name%type
    ) IS
    BEGIN
        update locations
        set location_name = upper(pi_location_name)
        where location_id = pi_location_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_update_location - pi_location_id: ' || pi_location_id
                || '; pi_location_name: ' || pi_location_name
                || chr(10) || sqlerrm);
    END p_update_location;
-------------------------------------------------------------------------
    PROCEDURE p_delete_location (
        pi_location_id  IN locations.location_id%type
    ) IS
    BEGIN
        update locations
        set deleted = sysdate
        where location_id = pi_location_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_delete_location - pi_location_id: ' || pi_location_id
                || chr(10) || sqlerrm);
    END p_delete_location;
-------------------------------------------------------------------------
END pkg_locations;
/
-------------------------------------------------------------------------